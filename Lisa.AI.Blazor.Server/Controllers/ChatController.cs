using Lisa.AI.OpenAIModels.ChatCompletionModels;
using Lisa.AI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lisa.AI.Blazor.Server.Controllers
{
    /// <summary>
    /// Chat Completion Controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;

        /// <summary>
        /// Initializes the Chat Completion Controller
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles Chat Completion Requests
        /// </summary>
        /// <param name="request">Chat completion request</param>
        /// <param name="service">LLM model service</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <remarks>
        /// Streaming is disabled by default. To enable, set `stream:true` explicitly.
        /// </remarks>
        /// <response code="200">Model chat completion result</response>
        /// <response code="400">Error message</response>
        [HttpPost("/v1/chat/completions")]
        [HttpPost("/chat/completions")]
        [HttpPost("/openai/deployments/{model}/chat/completions")]
        [Produces("text/event-stream")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatCompletionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IResult> CreateChatCompletionAsync(
            [FromBody] ChatCompletionRequest request,
            [FromServices] ILLmModelService service,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request.stream)
                {
                    string first = " ";
                    await foreach (var item in service.CreateChatCompletionStreamAsync(request, cancellationToken))
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
                    return Results.Ok(await service.CreateChatCompletionAsync(request, cancellationToken));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateChatCompletionAsync");
                return Results.Problem($"{ex.Message}");
            }
        }
    }
}
