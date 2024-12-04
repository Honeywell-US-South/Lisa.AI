namespace Lisa.AI.OpenAIModels.EmbeddingModels;

public class EmbeddingObject
{
    /// <summary>
    /// Index of the embedding in the list of embeddings.
    /// </summary>
    public int index { get; set; } = 0;

    /// <summary>
    /// Embedding vector, represented as a list of floats. The length of the vector depends on the model, as listed in the embedding guide.
    /// </summary>
    public IReadOnlyList<float[]> embedding { get; set; }

    /// <summary>
    /// Object type, always "embedding".
    /// </summary>
    public string @object { get; set; } = "embedding";
}
