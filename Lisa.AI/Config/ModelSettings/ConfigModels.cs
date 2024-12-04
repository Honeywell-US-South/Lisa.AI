namespace Lisa.AI.Config.ModelSettings;

/// <summary>
/// Configuration for Models
/// </summary>
public class ConfigModels
{
    /// <summary>
    /// Currently Used Model
    /// </summary>
    public int Current { get; set; }

    /// <summary>
    /// Whether the model is loaded
    /// </summary>
    public bool Loaded { get; set; }

    /// <summary>
    /// List of Models
    /// </summary>
    public List<LLmModelSettings>? Models { get; set; }
}
