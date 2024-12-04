using System.Text.Json.Serialization;

namespace Lisa.AI.OpenAIModels.ToolModels;

/// <summary>
/// Details of the function
/// </summary>
public class FunctionInfo
{
    /// <summary>
    /// Name of the function to be called. Must consist of a-z, A-Z, 0-9, underscores, or hyphens, with a maximum length of 64.
    /// </summary>
    public required string name { get; set; }

    /// <summary>
    /// Description of the function's purpose, used by the model to determine when and how to call the function.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? description { get; set; }

    /// <summary>
    /// Parameters accepted by the function, described as a JSON schema object.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Parameters? parameters { get; set; }
}
