namespace Lisa.AI.Config.ModelSettings;

/// <summary>
/// Model Transformation Parameters
/// </summary>
public class WithTransform
{
    /// <summary>
    /// Conversation Transformation
    /// </summary>
    public string? HistoryTransform { get; set; }

    /// <summary>
    /// Output Transformation
    /// </summary>
    public string? OutputTransform { get; set; }
}
