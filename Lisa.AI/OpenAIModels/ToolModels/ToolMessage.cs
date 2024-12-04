namespace Lisa.AI.OpenAIModels.ToolModels;

/// <summary>
/// Tool message
/// </summary>
public class ToolMessage
{
    /// <summary>
    /// ID of the tool call
    /// </summary>
    public string id { get; set; }

    /// <summary>
    /// Tool type, currently fixed to "function"
    /// </summary>
    public string type { get; set; } = "function";

    /// <summary>
    /// Information about the called function
    /// </summary>
    public ToolMessageFuntion function { get; set; }
}
