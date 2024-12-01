﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisa.AI.OpenAIModels
{
    /// <summary>
    /// Embedding Request
    /// Documentation: https://platform.openai.com/docs/api-reference/embeddings/create
    /// </summary>
    public class EmbeddingRequest
    {
        /// <summary>
        /// Input text for embedding, encoded as a string or an array of tokens. 
        /// To embed multiple inputs in a single request, pass a string array or an array of token arrays.
        /// Inputs cannot exceed the model's maximum input tokens (e.g., 8192 tokens), cannot be empty strings, 
        /// and any array must be 2048 dimensions or fewer.
        /// </summary>
        /// <example>My name is sangsq</example>
        public string[] input { get; set; } = Array.Empty<string>();

        /// <summary>
        /// ID of the model to use. You can use the list models API to see all available models 
        /// or refer to the model overview for descriptions.
        /// </summary>
        /// <example>default</example>
        public string model { get; set; } = string.Empty;

        /// <summary>
        /// Format of the returned embedding. Can be either float or base64.
        /// </summary>
        /// <example>float</example>
        public string encoding_format { get; set; } = "float";

        /// <summary>
        /// Dimensions that the output embedding should have. Supported only in text-embedding-3 and newer models.
        /// </summary>
        public int? dimensions { get; set; }

        /// <summary>
        /// A unique identifier representing your end user, which can help OpenAI monitor and detect abuse.
        /// </summary>
        public string? user { get; set; }
    }

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
}