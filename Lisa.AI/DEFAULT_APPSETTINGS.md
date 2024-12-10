{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  // API Key
  "ApiKey": "",
  "GlobalSettings": {
    "CurrentModelIndex": 0,
    // Auto release time, in minutes. 0 means no auto-release.
    "AutoReleaseTime": 30
  },
  // LLM Service Configuration
  "LLmModelSettings": [
    {
      "Name": "llama-3.2-3B",
      "Description": "llama-3.2-3B",
      "Version": "3.1",
      "WebSite": "https://ollama.com/library/llama3.2",
      "SystemPrompt": "You are a helpful assistant",
      "ModelParams": {
        "ModelPath": "C:\\Code\\models\\Llama-Magpie-3.2-3B-Instruct.F16.gguf",
        "ContextSize": 32768,
        "Seed": 1337,
        "GpuLayerCount": 30
      },
      "AntiPrompts": [ "<|eot_id|>", "<|endoftext|>" ],
      "WithTransform": {
        "HistoryTransform": "Lisa.AI.Transform.LLamaHistoryTransform"
      },
      "ToolPrompt": {
        "Index": 1,
        "Lang": "en"
      }
    }
  ],
  "ToolPromptConfig": [
    {
      "PromptConfigDesc": "default",
      "FN_NAME": "Action", // Function name identifier
      "FN_ARGS": "Action Input", // Parameter identifier
      "FN_RESULT": "Observation", // Result identifier
      "FN_EXIT": "Answer:", // Identifier after tool execution
      "FN_STOP_WORDS": [ "Observation:", "Answer:" ], // Special stop words used during tool invocation
      "FN_TEST": "Action:? ?(.*?)\\s*(Action Input:? ?(.*?)\\s*)(?=Observation|$|\\n)", // Regex for extracting function name (group 1) and parameters (group 3)
      "FN_CALL_TEMPLATE": "Action: {0}\nAction Input: {1}", // Tool invocation template
      "FN_RESULT_SPLIT": "", // Separator between tool invocation and result
      "FN_RESULT_TEMPLATE": "Observation: {0}", // Tool result template
      "FN_CALL_TEMPLATE_INFO": {
        "en": "# Tools\n\n## You have access to the following tools:\n\n{tool_descs}\n"
      },
      "FN_CALL_TEMPLATE_FMT": {
        "en": "## When you need to call a tool, please insert the following command in your reply, You can call zero or more times according to your needs:\n\nTool Invocation\n{0}: The name of the tool, must be one of [{4}]\n{1}: Tool input\n{2}: <result>Tool returns result</result>\n{3}: Summarize the results of this tool call based on Observation. If the result contains url,please display it in the following format:![Image](URL)"
      }, // 0 FN_NAME 1 FN_ARGS 2 FN_RESULT 3 FN_EXIT 4 toolNames
      "FN_CALL_TEMPLATE_FMT_PARA": {
        "en": "## When you need to call a tool, please intersperse the following tool command in your reply. You can call zero or more times according to your needs:\n\nTool Invocation\n{0}: The name of the tool 1, must be one of [{4}]\n{1}: Tool input to tool 1\n{0}: The name of the tool 2, must be one of [{4}]\n{1}: Tool input to tool 2\n...\n{0}: The name of the tool N, must be one of [{4}]\n{1}: Tool input to tool N\n{2}: <result>Tool 1 returns result</result>\n{2}: <result>Tool 2 returns result</result>\n...\n{2}: <result>Tool N returns result</result>\n{3}: Summarize the results of this tool call based on Observation. If the result contains url,please display it in the following format:![Image](URL)"
      },
      "ToolDescTemplate": {
        "en": "### {0}\n\n{1}: {2} Parameters: {3}"
      }
    },
    {
      "PromptConfigDesc": "Llama Template",
      "FN_NAME": "{\"name\":",
      "FN_ARGS": "\"parameters\":",
      "FN_RESULT": "",
      "FN_EXIT": "<|start_header_id|>assistant<|end_header_id|>",
      "FN_STOP_WORDS": [],
      "FN_TEST": "\\{\"name\" ?: ?\"(.*?)\",\\s*\"parameters\" ?: ?(\\{.*?\\}|\\{})",
      "FN_CALL_TEMPLATE": "{{\"name\":\"{0}\", \"parameters\":{1}}}",
      "FN_RESULT_SPLIT": "<|eot_id|>",
      "FN_RESULT_TEMPLATE": "<|start_header_id|>ipython<|end_header_id|>\n{0}<|eot_id|>",
      "FN_CALL_TEMPLATE_INFO": {
        "en": "You are a helpful assistant with tool calling capabilities. When you receive a tool call response, use the output to format an answer to the original use question.\n\nGiven the following functions, please respond with a JSON for a function call with its proper arguments that best answers the given prompt.\n\n{tool_descs}"
      },
      "FN_CALL_TEMPLATE_FMT": {
        "en": "When you need to call a tool, please insert the following command in your reply. Respond in the format `{0}` for the tool name and `{1}` for the dictionary of argument name and value. The function name of the tool must be one of [{4}]. Do not use variables or any tool names outside this list."
      },
      "FN_CALL_TEMPLATE_FMT_PARA": {
        "en": "## Insert the following commands in your reply when you need to call multiple tools in parallel:\n\n{0}: The name of tool 1, {1}: The input of tool 1}}\n{0}: The name of tool 2, {1}: The input of tool 2}}\n...\n{0}: The name of tool N, {1}: The input of tool N}}. Each tool name must be one of [{4}]. Do not use variables or any tool names outside this list."
      },
      "ToolDescTemplate": {
        "en": "{{\"name\": \"{1}\", \"parameters\":{3}}}"
      }
    }
  ],
  // Independent embedding service configuration
  "EmbedingForward": "http://127.0.0.1:5000/embeddings"
}
