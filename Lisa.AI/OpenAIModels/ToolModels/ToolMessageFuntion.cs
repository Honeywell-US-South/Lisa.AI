﻿namespace Lisa.AI.OpenAIModels.ToolModels;

/// <summary>
/// Response selection for invoking a tool
/// </summary>
public class ToolMessageFuntion
{
    /// <summary>
    /// Function name
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// Parameters for calling the function, generated by the model in JSON format.
    /// Note that the model does not always generate valid JSON and might hallucinate parameters not defined in the function schema.
    /// Validate parameters in code before invoking the function.
    /// </summary>
    public string? arguments { get; set; }
}
