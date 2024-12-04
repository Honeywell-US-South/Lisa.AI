namespace Lisa.AI.OpenAIModels.ToolModels;

/// <summary>
/// Tool message chunk for streaming processing
/// </summary>
public class ToolMessageChunk : ToolMessage
{
    /// <summary>
    /// Index of the tool call
    /// </summary>
    public int index { get; set; }
}
