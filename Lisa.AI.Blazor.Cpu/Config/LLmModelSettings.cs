using LLama.Common;

namespace Lisa.AI.Blazor.Config
{
    /// <summary>
    /// Model Configuration Information
    /// </summary>
    public class LLmModelSettings
    {
        /// <summary>
        /// Model Name
        /// </summary>
        public string Name { get; set; } = "default";

        /// <summary>
        /// Model Description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Model Website Information
        /// </summary>
        public string? WebSite { get; set; }

        /// <summary>
        /// Model Version
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Default configuration to use when no system prompt is specified during a conversation
        /// </summary>
        public string? SystemPrompt { get; set; }

        /// <summary>
        /// Model Loading Parameters
        /// </summary>
        public ModelParams ModelParams { get; set; }

        /// <summary>
        /// Model Transformation Parameters
        /// </summary>
        public WithTransform? WithTransform { get; set; }

        /// <summary>
        /// Stop Words
        /// </summary>
        public string[]? AntiPrompts { get; set; }

        /// <summary>
        /// Tool Prompt Configuration
        /// </summary>
        public ToolPromptInfo ToolPrompt { get; set; }

        /// <summary>
        /// Model Configuration Constructor
        /// </summary>
        /// <param name="modelPath">Path to the model</param>
        public LLmModelSettings(string modelPath)
        {
            ModelParams = new ModelParams(modelPath);
            ToolPrompt = new ToolPromptInfo();
        }

        /// <summary>
        /// Default Model Configuration Constructor
        /// </summary>
        public LLmModelSettings()
        {
            ModelParams = new ModelParams("");
            ToolPrompt = new ToolPromptInfo();
        }
    }

    /// <summary>
    /// Tool Prompt Configuration
    /// </summary>
    public class ToolPromptInfo
    {
        /// <summary>
        /// Prompt Template Index
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// Template Prompt Language
        /// </summary>
        public string Lang { get; set; } = "en";
    }

    /// <summary>
    /// Model Transformation Parameters
    /// </summary>
    public class WithTransform
    {
        /// <summary>
        /// Conversation Transformation
        /// </summary>
        public string? HistoryTransform { get; set; }

        /// <summary>
        /// Output Transformation
        /// </summary>
        public string? OutputTransform { get; set; }
    }

    /// <summary>
    /// Configuration for Models
    /// </summary>
    public class ConfigModels
    {
        /// <summary>
        /// Currently Used Model
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// Whether the model is loaded
        /// </summary>
        public bool Loaded { get; set; }

        /// <summary>
        /// List of Models
        /// </summary>
        public List<LLmModelSettings>? Models { get; set; }
    }
}
