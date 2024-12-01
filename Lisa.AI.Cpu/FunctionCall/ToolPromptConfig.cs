using System;
using System.Collections.Generic;

namespace Lisa.AI.FunctionCall
{
    /// <summary>
    /// Tool Prompt Configuration
    /// </summary>
    public class ToolPromptConfig
    {
        /// <summary>
        /// Description of the tool prompt configuration
        /// </summary>
        public string PromptConfigDesc { get; set; }

        /// <summary>
        /// Placeholder for the tool name
        /// </summary>
        public string FN_NAME { get; set; }

        /// <summary>
        /// Placeholder for tool parameters
        /// </summary>
        public string FN_ARGS { get; set; }

        /// <summary>
        /// Placeholder for tool results
        /// </summary>
        public string FN_RESULT { get; set; }

        /// <summary>
        /// Function call template
        /// </summary>
        public string FN_CALL_TEMPLATE { get; set; }

        /// <summary>
        /// Separator between function calls and results
        /// </summary>
        public string FN_RESULT_SPLIT { get; set; }

        /// <summary>
        /// Template for tool results
        /// </summary>
        public string FN_RESULT_TEMPLATE { get; set; }

        /// <summary>
        /// Regular expression for extracting function names and parameters
        /// </summary>
        public string FN_TEST { get; set; }

        /// <summary>
        /// Placeholder for tool return value
        /// </summary>
        public string FN_EXIT { get; set; }

        /// <summary>
        /// List of stop words for tools
        /// </summary>
        public string[] FN_STOP_WORDS { get; set; }

        /// <summary>
        /// Template information for tool descriptions, divided by language
        /// </summary>
        public Dictionary<string, string> FN_CALL_TEMPLATE_INFO { get; set; }

        /// <summary>
        /// Tool invocation template, divided by language
        /// </summary>
        public Dictionary<string, string> FN_CALL_TEMPLATE_FMT { get; set; }

        /// <summary>
        /// Parallel tool invocation template, divided by language
        /// </summary>
        public Dictionary<string, string> FN_CALL_TEMPLATE_FMT_PARA { get; set; }

        // TODO: Define tool description templates, divided by language

        /// <summary>
        /// Tool description templates, divided by language
        /// </summary>
        public Dictionary<string, string> ToolDescTemplate { get; set; }
    }
}
