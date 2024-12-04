namespace Lisa.AI.OpenAIModels.BaseCompletionModels;

/// <summary>
/// Base Completion Response Content Choice
/// </summary>
public class BaseCompletionResponseChoice
{
    /// <summary>
    /// Index of the choice in the list of choices
    /// </summary>
    public int index { get; set; }

    /// <summary>
    /// Reason why the model stopped generating tokens.
    /// This will be "stop" if the model reached a natural stopping point or the provided stop sequence,
    /// "length" if the maximum number of tokens specified in the request was reached, or "content_filter".
    /// </summary>
    public string? finish_reason { get; set; }
}
