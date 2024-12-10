using Lisa.AI.OpenAIModels.ToolModels;

namespace Lisa.AI.Services
{
    public interface IToolService
    {
        /// <summary>
        /// Checks if a tool is registered.
        /// </summary>
        /// <param name="toolName">The name of the tool.</param>
        /// <returns>True if the tool is registered, otherwise false.</returns>
        bool IsToolRegistered(string toolName);

        /// <summary>
        /// Registers a tool with its handler function.
        /// </summary>
        /// <param name="functionInfo">The function information for the tool.</param>
        /// <param name="toolFunction">The handler function for the tool.</param>
        void RegisterTool(FunctionInfo functionInfo, Func<Dictionary<string, object>, Task<object>> toolFunction);

        /// <summary>
        /// Unregisters a tool by its name.
        /// </summary>
        /// <param name="toolName">The name of the tool to unregister.</param>
        void UnregisterTool(string toolName);

        /// <summary>
        /// Updates an existing tool's definition and handler function.
        /// </summary>
        /// <param name="toolName">The name of the tool to update.</param>
        /// <param name="newFunctionInfo">The new function information for the tool.</param>
        /// <param name="newToolFunction">The new handler function for the tool.</param>
        void UpdateTool(string toolName, FunctionInfo newFunctionInfo, Func<Dictionary<string, object>, Task<object>> newToolFunction);

        /// <summary>
        /// Retrieves a list of all registered tools.
        /// </summary>
        /// <returns>A list of registered tools.</returns>
        List<ToolInfo> GetRegisteredTools();

        /// <summary>
        /// Executes a list of tool calls asynchronously.
        /// </summary>
        /// <param name="toolMessages">The list of tool messages to execute.</param>
        /// <returns>A list of tool responses.</returns>
        Task<List<ToolResponse>> ExecuteToolCallsAsync(List<ToolMessage> toolMessages);

        /// <summary>
        /// Validates a tool against its registered version.
        /// </summary>
        /// <param name="toolInfo">The tool information to validate.</param>
        /// <returns>True if the tool matches the registered version, otherwise false.</returns>
        bool ValidateTool(FunctionInfo toolInfo);
    }
}
