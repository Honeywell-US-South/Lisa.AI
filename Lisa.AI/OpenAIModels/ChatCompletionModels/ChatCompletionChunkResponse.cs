using Lisa.AI.OpenAIModels.BaseCompletionModels;

namespace Lisa.AI.OpenAIModels.ChatCompletionModels
{
    /// <summary>
    /// Chat Completion Response for Streaming
    /// Documentation: https://platform.openai.com/docs/api-reference/chat/streaming
    /// </summary>
    public class ChatCompletionChunkResponse : BaseCompletionResponse
    {
        /// <summary>
        /// Object type, always "chat.completion.chunk"
        /// </summary>
        public string @object = "chat.completion.chunk";

        /// <summary>
        /// List of chat completion choices. Multiple choices if n > 1.
        /// </summary>
        public ChatCompletionChunkResponseChoice[] choices { get; set; } = Array.Empty<ChatCompletionChunkResponseChoice>();
    }
}
