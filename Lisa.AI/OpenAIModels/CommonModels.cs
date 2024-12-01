﻿namespace Lisa.AI.OpenAIModels
{
    /// <summary>
    /// Token usage information for inference completion
    /// </summary>
    public class UsageInfo
    {
        /// <summary>
        /// Number of prompt tokens
        /// </summary>
        public int prompt_tokens { get; set; } = 0;

        /// <summary>
        /// Number of completion tokens
        /// </summary>
        public int completion_tokens { get; set; } = 0;

        /// <summary>
        /// Total number of tokens
        /// </summary>
        public int total_tokens { get; set; } = 0;
    }

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
}