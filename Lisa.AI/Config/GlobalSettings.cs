using Microsoft.Extensions.Configuration;

namespace Lisa.AI.Config;

/// <summary>
/// Global Settings
/// </summary>
public static class GlobalSettings
{
    /// <summary>
    /// Index of the initially loaded model
    /// </summary>
    public static int CurrentModelIndex { get; set; } = 0;

    /// <summary>
    /// Index of the initially loaded embedding
    /// </summary>
    public static int CurrentEmbeddingIndex { get; set; } = 0;

    /// <summary>
    /// Indicates whether the model has been successfully loaded
    /// </summary>
    public static bool IsModelLoaded { get; set; } = false;

    /// <summary>
    /// Indicates whether the embedded has been successfully loaded
    /// </summary>
    public static bool IsEmbeddingLoaded { get; set; } = false;

    /// <summary>
    /// Automatic model release time
    /// </summary>
    public static int AutoReleaseTime { get; set; } = 0;

    /// <summary>
    /// Initialize global settings
    /// </summary>
    /// <param name="configuration">Configuration information</param>
    public static void InitializeGlobalSettings(IConfiguration configuration)
    {
        var applicationSettings = configuration.GetSection("GlobalSettings");
        CurrentModelIndex = applicationSettings.GetValue<int>("CurrentModelIndex");
        CurrentEmbeddingIndex = applicationSettings.GetValue<int>("CurrentEmbeddingIndex");
        AutoReleaseTime = applicationSettings.GetValue<int>("AutoReleaseTime");
    }
}
