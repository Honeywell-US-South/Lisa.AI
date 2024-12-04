using Lisa.AI.OpenAIModels.CommonModels;

namespace Lisa.AI.OpenAIModels.BaseCompletionModels;

/// <summary>
/// Base Completion Response
/// </summary>
public class BaseCompletionResponse
{
    /// <summary>
    /// Unique identifier for the chat completion
    /// </summary>
    public string id { get; set; } = string.Empty;

    /// <summary>
    /// Unix timestamp (in seconds) when the chat completion was created
    /// </summary>
    public long created { get; set; }

    /// <summary>
    /// Model used for chat completion
    /// </summary>
    public string model { get; set; } = string.Empty;

    /// <summary>
    /// Usage statistics for the completion request
    /// </summary>
    public UsageInfo usage { get; set; } = new();
}
