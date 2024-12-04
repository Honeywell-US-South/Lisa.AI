using System.Text.Json.Serialization;

namespace Lisa.AI.OpenAIModels.ToolModels;

/// <summary>
/// Information about a parameter, used to describe function parameters
/// </summary>
public class ParameterInfo
{
    /// <summary>
    /// Parameter type
    /// </summary>
    public required string type { get; set; }

    /// <summary>
    /// Description of the parameter
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? description { get; set; }

    /// <summary>
    /// Optional values for the parameter
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? @enum { get; set; }

    /// <summary>
    /// Indicates whether the parameter is required
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? required { get; set; }
}
