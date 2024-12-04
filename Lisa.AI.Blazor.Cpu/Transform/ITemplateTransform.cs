using Lisa.AI.Blazor.Config;
using Lisa.AI.Blazor.FunctionCall;
using Lisa.AI.OpenAIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisa.AI.Blazor.Cpu.Transform
{
    public interface ITemplateTransform
    {
        public string HistoryToText(ChatCompletionMessage[] history, ToolPromptGenerator generator, ToolPromptInfo toolinfo, string toolPrompt);
    }
}
