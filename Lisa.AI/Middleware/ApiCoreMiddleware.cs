using Lisa.AI.Config;
using Lisa.AI.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Lisa.AI.Middleware;

public class ApiCoreMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpClient _client;
    private readonly List<NodeServer> _nodeServers;


    public ApiCoreMiddleware(RequestDelegate next, IConfiguration configuration, HttpClient client)
    {
        _next = next;
        _client = client;
        _nodeServers = new List<NodeServer>();
        var nodes = configuration.GetSection("NodeServers");
        // Extract NodeServers settings
        configuration.GetSection("NodeServers").Bind(_nodeServers);
        
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "POST")
        {
            string path = context.Request.Path.ToString();
            string action = "";
            if (path.Contains("visions", StringComparison.OrdinalIgnoreCase))
            {
                action = "gpt";
            }
            else if (path.Contains("completions", StringComparison.OrdinalIgnoreCase))
            {
                action = "gpt";
            }
            else if (context.Request.Path.ToString().Contains("embeddings", StringComparison.OrdinalIgnoreCase))
            {
                action = "embedding";
            }
            
            if (!string.IsNullOrEmpty(action))
            {
                bool processLocal = true;

                try
                {
                    var node = _nodeServers.SelectNode(action);
                    if (node != null)
                    {
                        try
                        {
                            
                            node.RegisterNodeUse(action);

                            var url = node.Url.Trim('/');

                            url = $"{node.Url.Trim('/')}/{path.Trim('/')}";
                            // Forward the request to the node server
                            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                            {
                                Content = new StringContent(await new StreamReader(context.Request.Body).ReadToEndAsync(), Encoding.UTF8, "application/json")
                            };

                            var responseMessage = await _client.SendAsync(requestMessage);

                            // Forward the response back to the client
                            context.Response.StatusCode = (int)responseMessage.StatusCode;
                            context.Response.ContentType = responseMessage.Content.Headers.ContentType.ToString();
                            await context.Response.WriteAsync(await responseMessage.Content.ReadAsStringAsync());
                            processLocal = false;
                            
                        } catch (Exception ex) {
                            processLocal = true;
                            Console.WriteLine(ex); 

                        }
                        finally {  node.UnregisterNodeUse(action); }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }

                if (processLocal)
                {
                    await ProcessLocal(context);
                    
                }

                //request handled by remote or local server
                // end request
                return; 
            }
                
        }

        await _next(context);
    }

    private async Task ProcessLocal(HttpContext context)
    {
        if (context.Request.Method == "POST")
        {
            if (context.Request.Path.ToString().Contains("completions") || context.Request.Path.ToString().Contains("embeddings"))
            {
                context.Request.EnableBuffering();

                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;

                try
                {
                    using (JsonDocument doc = JsonDocument.Parse(body))
                    {
                        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body);
                        if (data != null)
                        {
                            if (data.TryGetValue("stop", out var stop) && stop.ValueKind == JsonValueKind.String)
                            {
                                data["stop"] = JsonSerializer.Deserialize<JsonElement>(
                                    JsonSerializer.Serialize(new string[] { stop.ToString() })
                                );
                            }

                            if (data.TryGetValue("input", out var input) && input.ValueKind == JsonValueKind.String)
                            {
                                data["input"] = JsonSerializer.Deserialize<JsonElement>(
                                    JsonSerializer.Serialize(new string[] { input.ToString() })
                                );
                            }

                            var newBody = JsonSerializer.Serialize(data);
                            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(newBody));
                        }
                    }
                }
                catch
                {
                    // Let validation handle exceptions
                }
            }
        }

        await _next(context);
    }
}
