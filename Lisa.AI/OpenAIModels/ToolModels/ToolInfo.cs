namespace Lisa.AI.OpenAIModels.ToolModels;

/// <summary>
/// Information about tokens for inference completion
/// </summary>
public class ToolInfo
{
    /// <summary>
    /// Tool type, currently supports only "function"
    /// </summary>
    public string type { get; set; } = "function";

    /// <summary>
    /// Function details
    /// </summary>
    public required FunctionInfo function { get; set; }
}
