using Lisa.AI.OpenAIModels.BaseCompletionModels;

namespace Lisa.AI.OpenAIModels.CompletionModels;

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
