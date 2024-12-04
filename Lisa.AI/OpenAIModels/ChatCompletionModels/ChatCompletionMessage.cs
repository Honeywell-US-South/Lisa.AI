using Lisa.AI.OpenAIModels.ToolModels;
using System.Text.Json.Serialization;

namespace Lisa.AI.OpenAIModels.ChatCompletionModels;

/// <summary>
/// Chat Message List
/// </summary>
public class ChatCompletionMessage
{
    /// <summary>
    /// Role of the message
    /// Options: system, user, assistant, tool
    /// </summary>
    /// <example>user</example>
    public string? role { get; set; } = string.Empty;

    /// <summary>
    /// Content of the message
    /// </summary>
    /// <example>Hello</example>
    public string? content { get; set; }

    /// <summary>
    /// Information about tool calls
    /// </summary>
    /// <example>null</example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ToolMessage[]? tool_calls { get; set; }

    /// <summary>
    /// ID of the invoked tool
    /// Required when role is "tool"
    /// </summary>
    /// <example>null</example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? tool_call_id { get; set; }
}
