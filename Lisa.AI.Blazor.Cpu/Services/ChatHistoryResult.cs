using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisa.AI.Blazor.Cpu.Services
{
    /// <summary>
    /// Chat History Result
    /// </summary>
    public class ChatHistoryResult
    {
        /// <summary>
        /// Chat History
        /// </summary>
        public string ChatHistory { get; set; }

        /// <summary>
        /// Indicates whether the tool prompt is enabled
        /// </summary>
        public bool IsToolPromptEnabled { get; set; }

        /// <summary>
        /// Tool Stop Words
        /// </summary>
        public string[]? ToolStopWords { get; set; }

        /// <summary>
        /// Initializes a new instance of the ChatHistoryResult class
        /// </summary>
        /// <param name="chatHistory">Chat history</param>
        /// <param name="isToolPromptEnabled">Whether the tool prompt is enabled</param>
        /// <param name="toolStopWords">Tool stop words</param>
        public ChatHistoryResult(string chatHistory, bool isToolPromptEnabled, string[]? toolStopWords)
        {
            ChatHistory = chatHistory;
            IsToolPromptEnabled = isToolPromptEnabled;
            ToolStopWords = toolStopWords;
        }
    }
}
