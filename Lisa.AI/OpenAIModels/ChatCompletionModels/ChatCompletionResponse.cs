using Lisa.AI.OpenAIModels.BaseCompletionModels;

namespace Lisa.AI.OpenAIModels.ChatCompletionModels;

/// <summary>
/// Chat Completion Response
/// </summary>
public class ChatCompletionResponse : BaseCompletionResponse
{
    /// <summary>
    /// Object type, always "chat.completion"
    /// </summary>
    public string @object = "chat.completion";

    /// <summary>
    /// List of chat completion choices. Multiple choices if n > 1.
    /// </summary>
    public ChatCompletionResponseChoice[] choices { get; set; } = Array.Empty<ChatCompletionResponseChoice>();
}
