using Lisa.AI.OpenAIModels.ChatCompletionModels;
using Lisa.AI.OpenAIModels.ToolModels;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace Lisa.AI.FunctionCall;

/// <summary>
/// Base Tool Prompt Generator
/// </summary>
public class ToolPromptGenerator
{
    private readonly List<ToolPromptConfig> _config;
    private readonly string[] _nullWords = new string[] { "null", "{}", "[]" };

    /// <summary>
    /// Initializes the Tool Prompt Generator
    /// </summary>
    /// <param name="config">Tool configuration</param>
    public ToolPromptGenerator(IOptions<List<ToolPromptConfig>> config)
    {
        _config = config.Value;
    }

    /// <summary>
    /// Checks if a tool is active
    /// </summary>
    /// <param name="tokens">Tokens</param>
    /// <param name="tpl">Template index</param>
    /// <returns></returns>
    public bool IsToolActive(List<string> tokens, int tpl = 0)
    {
        return string.Join("", tokens).Trim().StartsWith(_config[tpl].FN_NAME);
    }

    /// <summary>
    /// Gets the tool stop words
    /// </summary>
    /// <param name="tpl">Template index</param>
    /// <returns></returns>
    public string[] GetToolStopWords(int tpl = 0)
    {
        return _config[tpl].FN_STOP_WORDS;
    }

    /// <summary>
    /// Gets the tool result split character
    /// </summary>
    /// <param name="tpl">Template index</param>
    /// <returns></returns>
    public string GetToolResultSplit(int tpl = 0)
    {
        return _config[tpl].FN_RESULT_SPLIT;
    }

    /// <summary>
    /// Gets the tool prompt configuration
    /// </summary>
    /// <param name="tpl">Template index</param>
    /// <returns></returns>
    public ToolPromptConfig GetToolPromptConfig(int tpl = 0)
    {
        return _config[tpl];
    }

    /// <summary>
    /// Generates a tool call
    /// </summary>
    /// <param name="tool">Tool message</param>
    /// <param name="tpl">Template index</param>
    /// <returns></returns>
    public string GenerateToolCall(ToolMessage tool, int tpl = 0)
    {
        return string.Format(_config[tpl].FN_CALL_TEMPLATE, tool.function.name, tool.function.arguments);
    }

    /// <summary>
    /// Generates the result of a tool call
    /// </summary>
    /// <param name="res">Tool call result</param>
    /// <param name="tpl">Template index</param>
    /// <returns></returns>
    public string GenerateToolCallResult(string? res, int tpl = 0)
    {
        return string.Format(_config[tpl].FN_RESULT_TEMPLATE, res);
    }

    /// <summary>
    /// Generates a tool inference result
    /// </summary>
    /// <param name="res">Tool inference result</param>
    /// <param name="tpl">Template index</param>
    /// <returns></returns>
    public string GenerateToolCallReturn(string? res, int tpl = 0)
    {
        return $"{_config[tpl].FN_EXIT}: {res}";
    }

    /// <summary>
    /// Checks and generates tool calls
    /// </summary>
    /// <param name="input">Inference output</param>
    /// <param name="tpl">Template index</param>
    /// <returns></returns>
    public List<ToolMessageFuntion> GenerateToolCall(string input, int tpl = 0)
    {
        string pattern = _config[tpl].FN_TEST;
        Regex regex = new Regex(pattern, RegexOptions.Singleline);
        MatchCollection matches = regex.Matches(input);
        List<ToolMessageFuntion> results = new();
        foreach (Match match in matches)
        {
            string functionName = match.Groups[1].Value;
            string arguments = match.Groups[3].Success ? match.Groups[3].Value : "";
            if (string.IsNullOrWhiteSpace(arguments) || _nullWords.Contains(arguments))
            {
                arguments = null;
            }
            results.Add(new ToolMessageFuntion
            {
                name = functionName,
                arguments = arguments,
            });
        }
        return results;
    }

    /// <summary>
    /// Generates tool prompts
    /// </summary>
    /// <param name="req">Original chat generation request</param>
    /// <param name="tpl">Template index</param>
    /// <param name="lang">Language</param>
    /// <returns></returns>
    public string GenerateToolPrompt(ChatCompletionRequest req, int tpl = 0, string lang = "en")
    {
        // Return empty if no tools or the tool choice is "none"
        if (req.tools == null || req.tools.Length == 0 || (req.tool_choice != null && req.tool_choice.ToString() == "none"))
        {
            return string.Empty;
        }

        var config = _config[tpl];

        var toolDescriptions = req.tools.Select(tool => GetFunctionDescription(tool.function, config.ToolDescTemplate[lang])).ToArray();
        var toolNames = string.Join(",", req.tools.Select(tool => tool.function.name));

        var toolDescTemplate = config.FN_CALL_TEMPLATE_INFO[lang];
        var toolDesc = string.Join("\n\n", toolDescriptions);
        var toolSystem = toolDescTemplate.Replace("{tool_descs}", toolDesc);

        var parallelFunctionCalls = req.tool_choice?.ToString() == "parallel";
        var toolTemplate = parallelFunctionCalls ? config.FN_CALL_TEMPLATE_FMT_PARA[lang] : config.FN_CALL_TEMPLATE_FMT[lang];
        var toolPrompt = string.Format(toolTemplate, config.FN_NAME, config.FN_ARGS, config.FN_RESULT, config.FN_EXIT, toolNames);
        return $"\n\n{toolSystem}\n\n{toolPrompt}";
    }

    /// <summary>
    /// Generates a function description
    /// </summary>
    /// <param name="function">Function information</param>
    /// <param name="toolDescTemplate">Function template</param>
    /// <returns>Function description text</returns>
    private string GetFunctionDescription(FunctionInfo function, string toolDescTemplate)
    {
        // Return serialized function info if no description template
        if (string.IsNullOrWhiteSpace(toolDescTemplate))
        {
            return JsonSerializer.Serialize(new
            {
                type = "function",
                function = function
            }, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });
        }

        var nameForHuman = function.name;
        var nameForModel = function.name;
        var descriptionForModel = function.description ?? string.Empty;

        // Handle functions without parameters
        if (function.parameters == null || function.parameters.properties == null || function.parameters.properties.Count == 0)
        {
            return string.Format(toolDescTemplate, nameForHuman, nameForModel, descriptionForModel, string.Empty).Trim();
        }

        // Handle "required" fields in parameters
        var properties = function.parameters.properties;
        if (function.parameters.required?.Length > 0)
        {
            foreach (var key in function.parameters.required)
            {
                if (properties.ContainsKey(key))
                {
                    properties[key].required = true;
                }
            }
        }
        var parameters = JsonSerializer.Serialize(properties, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) });
        return string.Format(toolDescTemplate, nameForHuman, nameForModel, descriptionForModel, parameters).Trim();
    }
}
