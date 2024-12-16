using LLama.Common;

namespace Lisa.AI.Config.ModelSettings;

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
    /// Finetune model path
    /// </summary>
    public string? LoRAPath { get; set; }

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
