using Lisa.AI.OpenAIModels.CommonModels;

namespace Lisa.AI.OpenAIModels.EmbeddingModels;

/// <summary>
/// Response containing embeddings
/// </summary>
public class EmbeddingResponse
{
    /// <summary>
    /// Object type, always "list".
    /// </summary>
    public string @object { get; set; } = "list";

    /// <summary>
    /// List of embedding objects.
    /// </summary>
    public EmbeddingObject[] data { get; set; }

    /// <summary>
    /// Model used for embedding.
    /// </summary>
    public string model { get; set; } = string.Empty;

    /// <summary>
    /// Usage statistics.
    /// </summary>
    public EmbeddingUsageInfo usage { get; set; } = new EmbeddingUsageInfo();
}
