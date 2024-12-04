using Lisa.AI.OpenAIModels;

namespace Lisa.AI.Blazor.Cpu.Services
{
    /// <summary>
    /// Interface for LLM Model Service
    /// </summary>
    public interface ILLmModelService : IDisposable
    {
        /// <summary>
        /// Indicates whether embedding is supported
        /// </summary>
        bool IsSupportEmbedding { get; }

        /// <summary>
        /// Initialize the specified model
        /// </summary>
        void InitModelIndex();

        /// <summary>
        /// Explicitly release model resources
        /// </summary>
        void DisposeModel();

        /// <summary>
        /// Retrieve model information
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<string, string> GetModelInfo();

        /// <summary>
        /// Complete a chat session
        /// </summary>
        /// <param name="request">Chat completion request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<ChatCompletionResponse> CreateChatCompletionAsync(ChatCompletionRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Streamed generation for chat completion
        /// </summary>
        /// <param name="request">Chat completion request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        IAsyncEnumerable<string> CreateChatCompletionStreamAsync(ChatCompletionRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Create embeddings
        /// </summary>
        /// <param name="request">Embedding request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Word embeddings</returns>
        Task<EmbeddingResponse> CreateEmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Prompt completion
        /// </summary>
        Task<CompletionResponse> CreateCompletionAsync(CompletionRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Streamed generation for prompt completion
        /// </summary>
        /// <param name="request">Prompt completion request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        IAsyncEnumerable<string> CreateCompletionStreamAsync(CompletionRequest request, CancellationToken cancellationToken);
    }
}
