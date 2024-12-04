namespace Lisa.AI.OpenAIModels.CommonModels;

/// <summary>
/// Token usage information for embedding completion
/// </summary>
public class EmbeddingUsageInfo
{
    /// <summary>
    /// Number of prompt tokens
    /// </summary>
    public int prompt_tokens { get; set; } = 0;

    /// <summary>
    /// Total number of tokens
    /// </summary>
    public int total_tokens { get; set; } = 0;
}
