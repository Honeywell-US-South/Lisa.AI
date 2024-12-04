using Lisa.AI.Blazor.Config;
using Lisa.AI.Blazor.FunctionCall;
using Lisa.AI.OpenAIModels;
using System.Text;

namespace Lisa.AI.Blazor.Cpu.Transform
{
    /// <summary>
    /// ChatML History Transformation
    /// </summary>
    public class BaseHistoryTransform : ITemplateTransform
    {
        /// <summary>
        /// User token
        /// </summary>
        protected virtual string userToken => "<|im_start|>user";

        /// <summary>
        /// Assistant token
        /// </summary>
        protected virtual string assistantToken => "<|im_start|>assistant";

        /// <summary>
        /// System token
        /// </summary>
        protected virtual string systemToken => "<|im_start|>system";

        /// <summary>
        /// End token
        /// </summary>
        protected virtual string endToken => "<|im_end|>";

        /// <summary>
        /// Tracks function call information
        /// </summary>
        private Dictionary<string, string> functionCalls = new Dictionary<string, string>();

        /// <summary>
        /// Converts chat history into text
        /// </summary>
        /// <param name="history">Chat history messages</param>
        /// <param name="generator">Tool prompt generator</param>
        /// <param name="toolinfo">Tool prompt configuration</param>
        /// <param name="toolPrompt">Tool prompt content</param>
        /// <returns>Formatted history text</returns>
        public virtual string HistoryToText(ChatCompletionMessage[] history, ToolPromptGenerator generator, ToolPromptInfo toolinfo, string toolPrompt = "")
        {
            // System message to prepend when needed
            var systemMessage = "";

            StringBuilder sb = new();

            // Flags for tool call states
            bool toolWait = false;
            bool systemAdded = false;

            foreach (var message in history)
            {
                switch (message.role)
                {
                    case "user":
                        if (toolWait)
                        {
                            // Handle case where tool call is active but not completed
                            functionCalls.Clear();
                            toolWait = false;
                            sb.AppendLine(endToken);
                        }
                        sb.AppendLine($"{userToken}\n{systemMessage}{message.content}{endToken}");
                        systemMessage = "";
                        break;

                    case "system":
                        if (systemAdded || toolWait) continue;
                        systemAdded = true;

                        if (string.IsNullOrWhiteSpace(systemToken))
                        {
                            systemMessage = $"{message.content} {toolPrompt}";
                        }
                        else
                        {
                            sb.AppendLine($"{systemToken}\n{message.content}{toolPrompt}{endToken}");
                        }
                        break;

                    case "assistant":
                        if (toolWait)
                        {
                            foreach (var call in functionCalls)
                            {
                                sb.AppendLine(call.Value);
                            }
                            functionCalls.Clear();
                            var toolCallReturn = generator.GenerateToolCallReturn(message.content, toolinfo.Index);
                            sb.AppendLine($"{toolCallReturn}{endToken}");
                            toolWait = false;
                        }
                        else if (message.tool_calls?.Length > 0)
                        {
                            sb.AppendLine($"{assistantToken}");
                            foreach (var toolCall in message.tool_calls)
                            {
                                var toolCallPrompt = generator.GenerateToolCall(toolCall, toolinfo.Index);
                                sb.AppendLine($"{toolCallPrompt}");
                                functionCalls.Add(toolCall.id, "");
                            }
                            sb.Append(generator.GetToolResultSplit(toolinfo.Index));
                            toolWait = true;
                        }
                        else
                        {
                            sb.AppendLine($"{assistantToken}\n{message.content}{endToken}");
                        }
                        break;

                    case "tool":
                        if (message.tool_call_id is null || !toolWait || !functionCalls.ContainsKey(message.tool_call_id)) continue;
                        var toolCallResult = generator.GenerateToolCallResult(message.content, toolinfo.Index);
                        functionCalls[message.tool_call_id] = toolCallResult;
                        break;
                }
            }

            // Add inference or final assistant prompt
            var lastMessage = history.LastOrDefault();
            if (lastMessage?.role == "tool" && functionCalls.Count > 0)
            {
                foreach (var call in functionCalls)
                {
                    sb.AppendLine(call.Value);
                }
                functionCalls.Clear();
                sb.AppendLine(generator.GetToolPromptConfig(toolinfo.Index).FN_EXIT);
            }
            else if (toolWait)
            {
                sb.AppendLine($"{endToken}{assistantToken}");
            }
            else
            {
                sb.AppendLine(assistantToken);
            }

            return sb.ToString();
        }
    }
}
