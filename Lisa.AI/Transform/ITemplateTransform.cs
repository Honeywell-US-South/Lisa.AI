using Lisa.AI.Config.ModelSettings;
using Lisa.AI.FunctionCall;
using Lisa.AI.OpenAIModels.ChatCompletionModels;

namespace Lisa.AI.Blazor.Cpu.Transform
{
    public interface ITemplateTransform
    {
        public string HistoryToText(ChatCompletionMessage[] history, ToolPromptGenerator generator, ToolPromptInfo toolinfo, string toolPrompt);
    }
}
