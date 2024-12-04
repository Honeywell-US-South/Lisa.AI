using Lisa.AI.Config;
using Lisa.AI.Config.ModelSettings;
using Lisa.AI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lisa.AI.Blazor.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LLMController : ControllerBase
    {
        private readonly ILogger<LLMController> _logger;
        private readonly List<LLmModelSettings> _settings;

        public LLMController(ILogger<LLMController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _settings = configuration.GetSection(nameof(LLmModelSettings)).Get<List<LLmModelSettings>>();
        }

        /// <summary>
        /// Returns the basic information of the model
        /// </summary>
        /// <param name="service">LLM model service</param>
        /// <remarks>
        /// Retrieves metadata information about the model
        /// </remarks>
        /// <returns>JSON result containing metadata</returns>
        [HttpGet("/models/info")]
        public JsonResult GetModels([FromServices] ILLmModelService service)
        {
            var json = service.GetModelInfo();
            return new JsonResult(json);
        }

        /// <summary>
        /// Returns the configured model information
        /// </summary>
        [HttpGet("/models/config")]
        public ConfigModels GetConfigModels()
        {
            return new ConfigModels
            {
                Models = _settings,
                Loaded = GlobalSettings.IsModelLoaded,
                Current = GlobalSettings.CurrentModelIndex
            };
        }

        /// <summary>
        /// Switches to the specified model
        /// </summary>
        /// <param name="modelId">The ID of the model to switch to</param>
        [HttpPut("/models/{modelId}/switch")]
        public IActionResult SwitchModel(int modelId)
        {
            if (modelId < 0 || modelId >= _settings.Count)
            {
                return BadRequest("Invalid model ID");
            }

            // Save the current model index
            int index = GlobalSettings.CurrentModelIndex;

            if (GlobalSettings.CurrentModelIndex == modelId)
            {
                return Ok();
            }

            try
            {
                GlobalSettings.CurrentModelIndex = modelId;
                var service = HttpContext.RequestServices.GetRequiredService<ILLmModelService>();
                service.InitModelIndex();
            }
            catch (Exception e)
            {
                GlobalSettings.CurrentModelIndex = index;
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
