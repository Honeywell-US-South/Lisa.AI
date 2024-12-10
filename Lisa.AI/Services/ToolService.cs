using Lisa.AI.OpenAIModels.ToolModels;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Lisa.AI.Services
{
    public class ToolService : IToolService
    {
        private readonly ConcurrentDictionary<string, ToolFunctionDefinition> _tools = new();

        /// <summary>
        /// Checks if a tool is registered.
        /// </summary>
        public bool IsToolRegistered(string toolName) => _tools.ContainsKey(toolName);

        /// <summary>
        /// Registers a tool along with its handler function.
        /// </summary>
        public void RegisterTool(FunctionInfo functionInfo, Func<Dictionary<string, object>, Task<object>> toolFunction)
        {
            _tools.TryAdd(functionInfo.name, new ToolFunctionDefinition
            {
                Handler = toolFunction,
                FunctionInfo = functionInfo
            });
        }

        /// <summary>
        /// Unregisters a tool by its name.
        /// </summary>
        public void UnregisterTool(string toolName)
        {
            if (!_tools.TryRemove(toolName, out _))
            {
                throw new KeyNotFoundException($"Tool '{toolName}' is not registered.");
            }
        }

        /// <summary>
        /// Updates a registered tool's definition and handler.
        /// </summary>
        public void UpdateTool(string toolName, FunctionInfo newFunctionInfo, Func<Dictionary<string, object>, Task<object>> newToolFunction)
        {
            if (!_tools.ContainsKey(toolName))
            {
                throw new KeyNotFoundException($"Tool '{toolName}' is not registered.");
            }

            _tools[toolName] = new ToolFunctionDefinition
            {
                Handler = newToolFunction,
                FunctionInfo = newFunctionInfo
            };
        }

        /// <summary>
        /// Retrieves all registered tools.
        /// </summary>
        public List<ToolInfo> GetRegisteredTools()
        {
            return _tools.Select(tool => new ToolInfo
            {
                function = new FunctionInfo
                {
                    name = tool.Key,
                    description = tool.Value.FunctionInfo.description,
                    parameters = new Parameters
                    {
                        type = "object",
                        required = tool.Value.FunctionInfo.parameters.required,
                        properties = tool.Value.FunctionInfo.parameters.properties
                    }
                },
                type = "function"
            }).ToList();
        }

        /// <summary>
        /// Executes tool calls asynchronously and returns their responses.
        /// </summary>
        public async Task<List<ToolResponse>> ExecuteToolCallsAsync(List<ToolMessage> toolMessages)
        {
            var responses = new ConcurrentBag<ToolResponse>();

            await Task.WhenAll(toolMessages.Select(async toolMessage =>
            {
                if (_tools.TryGetValue(toolMessage.function.name, out var toolFunction))
                {
                    try
                    {
                        var arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(toolMessage.function.arguments ?? "{}");
                        var result = await toolFunction.Handler(arguments);

                        responses.Add(new ToolResponse
                        {
                            Id = toolMessage.id,
                            Type = toolMessage.type,
                            Result = result
                        });
                    }
                    catch (Exception ex)
                    {
                        responses.Add(new ToolResponse
                        {
                            Id = toolMessage.id,
                            Type = toolMessage.type,
                            Error = $"Error executing tool '{toolMessage.function.name}': {ex.Message}"
                        });
                    }
                }
                else
                {
                    responses.Add(new ToolResponse
                    {
                        Id = toolMessage.id,
                        Type = toolMessage.type,
                        Error = $"Tool '{toolMessage.function.name}' not registered."
                    });
                }
            }));

            return responses.ToList();
        }

        /// <summary>
        /// Validates a tool against the registered version.
        /// </summary>
        public bool ValidateTool(FunctionInfo toolInfo)
        {
            return _tools.TryGetValue(toolInfo.name, out var existingTool) &&
                   JsonSerializer.Serialize(existingTool.FunctionInfo.parameters) == JsonSerializer.Serialize(toolInfo.parameters);
        }
    }
}
