using Lisa.AI.OpenAIModels.EmbeddingModels;
using Lisa.AI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Lisa.AI.Blazor.Server.Controllers
{
    /// <summary>
    /// Embedding Controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class EmbeddingController : ControllerBase
    {
        private readonly ILogger<EmbeddingController> _logger;
        private readonly ILLmModelService _modelService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes the Embedding Controller
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="modelService">LLM Model Service</param>
        /// <param name="configuration">Configuration Service</param>
        /// <param name="client">HttpClient</param>
        public EmbeddingController(ILogger<EmbeddingController> logger, ILLmModelService modelService, IConfiguration configuration, HttpClient client)
        {
            _logger = logger;
            _modelService = modelService;
            _configuration = configuration;
            _client = client;
        }

        /// <summary>
        /// Creates an embedding
        /// </summary>
        /// <param name="request">Embedding request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Embedding response</returns>
        [HttpPost("/v1/embeddings")]
        [HttpPost("/embeddings")]
        [HttpPost("/openai/deployments/{model}/embeddings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmbeddingResponse))]
        public async Task<IResult> CreateEmbeddingAsync([FromBody] EmbeddingRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    return Results.BadRequest("Request is null");
                }

                // Create embeddings using the model service
                if (_modelService.IsSupportEmbedding)
                {
                    var response = await _modelService.CreateEmbeddingAsync(request, cancellationToken);
                    return Results.Ok(response);
                }
                else
                {
                    // Forward the request if embeddings are not supported
                    var url = _configuration["EmbedingForward"];
                    if (string.IsNullOrEmpty(url))
                    {
                        return Results.BadRequest("EmbedingForward is null");
                    }

                    var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                    var response = await _client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(result);
                        return Results.Ok(embeddingResponse);
                    }
                    else
                    {
                        return Results.BadRequest(response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateEmbeddingAsync");
                return Results.Problem($"{ex.Message}");
            }
        }
    }
}
