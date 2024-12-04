using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace Lisa.AI.Middleware;

public class TypeConversionMiddleware
{
    private readonly RequestDelegate _next;

    public TypeConversionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
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
