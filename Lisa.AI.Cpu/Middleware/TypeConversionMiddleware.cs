using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lisa.AI.Middleware
{
    /// <summary>
    /// Type Conversion Middleware
    /// </summary>
    public class TypeConversionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Handles the `stop` field in Completions requests
        /// </summary>
        /// <param name="next">Next middleware delegate</param>
        public TypeConversionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Asynchronous invocation
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>A task representing the asynchronous operation</returns>
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
                        // Handle potential JSON parsing exceptions
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
                        // Ignore exceptions, let validation handle them
                    }
                }
            }

            await _next(context);
        }
    }
}
