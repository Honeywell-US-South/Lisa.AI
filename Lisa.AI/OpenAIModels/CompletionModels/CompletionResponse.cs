using Lisa.AI.OpenAIModels.BaseCompletionModels;

namespace Lisa.AI.OpenAIModels.CompletionModels;

/// <summary>
/// Completion Response
/// </summary>
public class CompletionResponse : BaseCompletionResponse
{
    /// <summary>
    /// Object type, always "text_completion"
    /// </summary>
    public string @object = "text_completion";

    /// <summary>
    /// List of completion choices. Can have multiple choices if n > 1.
    /// </summary>
    public CompletionResponseChoice[] choices { get; set; } = Array.Empty<CompletionResponseChoice>();
}
