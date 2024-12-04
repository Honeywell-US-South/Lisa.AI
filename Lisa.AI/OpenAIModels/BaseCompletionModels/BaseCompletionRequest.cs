namespace Lisa.AI.OpenAIModels.BaseCompletionModels;

/// <summary>
/// Base Completion Request
/// </summary>
public class BaseCompletionRequest
{
    /// <summary>
    /// Model name
    /// </summary>
    /// <example>gpt</example>
    public string model { get; set; } = string.Empty;

    /// <summary>
    /// Number of chat completion options to generate for each input message.
    /// </summary>
    public int? n { get; set; } = 1;

    /// <summary>
    /// Temperature: Controls randomness. Lower values make the model's responses more repetitive and deterministic.
    /// Higher values lead to more unexpected or creative responses.
    /// Adjust either temperature or Top P, but not both at the same time.
    /// </summary>
    /// <example>1.0</example>
    public float temperature { get; set; } = 1.0f;

    /// <summary>
    /// Top P: Similar to temperature, this controls randomness but in a different way.
    /// Lowering Top P narrows the model's token selection range, making it more likely to choose high-probability tokens.
    /// Increasing Top P allows the model to choose from both high and low-probability tokens.
    /// Adjust either temperature or Top P, but not both at the same time.
    /// </summary>
    /// <example>1.0</example>
    public float top_p { get; set; } = 1.0f;

    /// <summary>
    /// Streamed response, default is false. 
    /// If set to true, the server responds with an HTTP stream response.
    /// </summary>
    /// <example>false</example>
    public bool stream { get; set; } = false;

    /// <summary>
    /// Stop sequences: Stops the model's response at desired points.
    /// The model's response will end before the specified sequences, so they won't appear in the text.
    /// For ChatGPT, using <|im_end|> ensures the model doesn't generate subsequent user queries.
    /// Up to four stop sequences can be provided.
    /// Can be string/array/null, middleware will handle as an array.
    /// </summary>
    /// <example>null</example>
    public string[]? stop { get; set; } = null;

    /// <summary>
    /// Maximum generation length: Sets a limit on the number of tokens per model response.
    /// The API supports up to MaxTokensPlaceholderDoNotTranslate tokens,
    /// shared across the prompt (including system messages, examples, message history, and user queries) and model responses.
    /// A token is approximately 4 characters of typical English text.
    /// </summary>
    /// <example>512</example>
    public int? max_tokens { get; set; } = null;

    /// <summary>
    /// Presence penalty: Reduces the probability of repeated tokens based on how often they appear in the text so far.
    /// This decreases the chance of repeating identical text in responses.
    /// </summary>
    public float presence_penalty { get; set; } = 0.0f;

    /// <summary>
    /// Frequency penalty: Reduces the likelihood of repeating any tokens present in the text so far.
    /// This increases the chance of introducing new topics in responses.
    /// </summary>
    public float frequency_penalty { get; set; } = 0.0f;

    /// <summary>
    /// Random seed: Used to control the randomness of model generation.
    /// If specified, the system will make best efforts for deterministic sampling, so repeated requests with the same seed and parameters should return the same results.
    /// </summary>
    /// <example>null</example>
    public uint? seed { get; set; }

    /// <summary>
    /// User: Unique identifier of the end user
    /// </summary>
    public string? user { get; set; }
}




