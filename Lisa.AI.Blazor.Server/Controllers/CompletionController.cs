using Lisa.AI.OpenAIModels.CompletionModels;
using Lisa.AI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lisa.AI.Blazor.Server.Controllers;

/// <summary>
/// Completion Controller
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class CompletionController : ControllerBase
{
    private readonly ILogger<CompletionController> _logger;

    /// <summary>
    /// Initializes the Completion Controller
    /// </summary>
    /// <param name="logger">Logger instance</param>
    public CompletionController(ILogger<CompletionController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles Completion Requests
    /// </summary>
    /// <param name="request">Completion request</param>
    /// <param name="service">LLM model service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <remarks>
    /// Streaming is disabled by default. To enable, set `stream:true` explicitly.
    /// </remarks>
    /// <response code="200">Model completion result</response>
    /// <response code="400">Error message</response>
    [HttpPost("/v1/completions")]
    [HttpPost("/completions")]
    [HttpPost("/openai/deployments/{model}/completions")]
    [Produces("text/event-stream")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompletionResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IResult> CreateCompletionAsync([FromBody] CompletionRequest request, [FromServices] ILLmModelService service, CancellationToken cancellationToken)
    {
        try
        {
            if (request.stream)
            {
                string first = " ";
                await foreach (var item in service.CreateCompletionStreamAsync(request, cancellationToken))
                {
                    if (first == " ")
                    {
                        first = item;
                    }
                    else
                    {
                        if (first.Length > 1)
                        {
                            Response.Headers.ContentType = "text/event-stream";
                            Response.Headers.CacheControl = "no-cache";
                            await Response.Body.FlushAsync();
                            await Response.WriteAsync(first);
                            await Response.Body.FlushAsync();
                            first = "";
                        }
                        await Response.WriteAsync(item);
                        await Response.Body.FlushAsync();
                    }
                }
                return Results.Empty;
            }
            else
            {
                return Results.Ok(await service.CreateCompletionAsync(request, cancellationToken));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateCompletionAsync");
            return Results.Problem($"{ex.Message}");
        }
    }
}
