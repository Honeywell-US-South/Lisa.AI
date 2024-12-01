﻿using System;

namespace Lisa.AI.OpenAIModels
{
    /// <summary>
    /// Completion Request
    /// Documentation: https://platform.openai.com/docs/api-reference/completions
    /// </summary>
    public class CompletionRequest : BaseCompletionRequest
    {
        /// <summary>
        /// Prompt
        /// The prompt to generate completions for, which can be a string, an array of strings, a token array, or an array of token arrays.
        /// </summary>
        /// <example>Today's weather is great,</example>
        public string prompt { get; set; } = string.Empty;
    }

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

    /// <summary>
    /// A single choice in the completion response
    /// </summary>
    public class CompletionResponseChoice : BaseCompletionResponseChoice
    {
        /// <summary>
        /// Text generated by the model for the completion
        /// </summary>
        public string? text { get; set; } = string.Empty;
    }
}