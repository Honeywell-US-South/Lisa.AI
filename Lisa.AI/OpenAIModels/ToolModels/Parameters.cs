using System.Text.Json.Serialization;

namespace Lisa.AI.OpenAIModels.ToolModels;

/// <summary>
/// Parameters of the function
/// </summary>
public class Parameters
{
    /// <summary>
    /// Parameter type, currently supports only "object"
    /// </summary>
    public string type { get; set; } = "object";

    /// <summary>
    /// Properties of the parameters, described as a JSON schema object.
    /// The keys are of type ParameterInfo.
    /// </summary>
    public required Dictionary<string, ParameterInfo> properties { get; set; }

    /// <summary>
    /// List of required parameter names.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? required { get; set; }
}
