using Lisa.AI.OpenAIModels.BaseCompletionModels;
using Lisa.AI.OpenAIModels.ToolModels;

namespace Lisa.AI.OpenAIModels.ChatCompletionModels;

/// <summary>
/// Chat Completion Request
/// Documentation: https://platform.openai.com/docs/api-reference/chat/create
/// </summary>
public class ChatCompletionRequest : BaseCompletionRequest
{
    /// <summary>
    /// Chat history
    /// </summary>
    public ChatCompletionMessage[] messages { get; set; } = Array.Empty<ChatCompletionMessage>();

    /// <summary>
    /// Controls whether and how the model invokes a tool.
    /// Can be a string ("none", "auto", "required", "parallel") or an object specifying a tool.
    /// {"type": "function", "function": {"name": "my_function"}}
    /// Default is "none", meaning the model generates a message without calling any tools.
    /// If tools are present, the default is "auto".
    /// - "required" means one or more tools must be invoked.
    /// - "parallel" means multiple tools are invoked in parallel.
    /// </summary>
    /// <example>null</example>
    public object? tool_choice { get; set; }

    /// <summary>
    /// List of tools the model might invoke. Currently, only functions are supported as tools.
    /// Provides a list of functions the model can generate JSON input for.
    /// Supports up to 128 functions.
    /// </summary>
    /// <example>null</example>
    public ToolInfo[]? tools { get; set; }
} 
