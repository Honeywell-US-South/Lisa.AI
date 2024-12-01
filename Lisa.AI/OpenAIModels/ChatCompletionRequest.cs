﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lisa.AI.OpenAIModels
{
    /// <summary>
    /// Chat Completion Request
    /// Documentation: https://platform.openai.com/docs/api-reference/chat/create
    /// </summary>
    public class ChatCompletionRequest : BaseCompletionRequest
    {
        /// <summary>
        /// Chat history
        /// </summary>
        public ChatCompletionMessage[] messages { get; set; } = Array.Empty<ChatCompletionMessage>();

        /// <summary>
        /// Controls whether and how the model invokes a tool.
        /// Can be a string ("none", "auto", "required", "parallel") or an object specifying a tool.
        /// {"type": "function", "function": {"name": "my_function"}}
        /// Default is "none", meaning the model generates a message without calling any tools.
        /// If tools are present, the default is "auto".
        /// - "required" means one or more tools must be invoked.
        /// - "parallel" means multiple tools are invoked in parallel.
        /// </summary>
        /// <example>null</example>
        public object? tool_choice { get; set; }

        /// <summary>
        /// List of tools the model might invoke. Currently, only functions are supported as tools.
        /// Provides a list of functions the model can generate JSON input for.
        /// Supports up to 128 functions.
        /// </summary>
        /// <example>null</example>
        public ToolInfo[]? tools { get; set; }
    }

    /// <summary>
    /// Chat Message List
    /// </summary>
    public class ChatCompletionMessage
    {
        /// <summary>
        /// Role of the message
        /// Options: system, user, assistant, tool
        /// </summary>
        /// <example>user</example>
        public string? role { get; set; } = string.Empty;

        /// <summary>
        /// Content of the message
        /// </summary>
        /// <example>Hello</example>
        public string? content { get; set; }

        /// <summary>
        /// Information about tool calls
        /// </summary>
        /// <example>null</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ToolMessage[]? tool_calls { get; set; }

        /// <summary>
        /// ID of the invoked tool
        /// Required when role is "tool"
        /// </summary>
        /// <example>null</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? tool_call_id { get; set; }
    }

    /// <summary>
    /// Chat Completion Response
    /// </summary>
    public class ChatCompletionResponse : BaseCompletionResponse
    {
        /// <summary>
        /// Object type, always "chat.completion"
        /// </summary>
        public string @object = "chat.completion";

        /// <summary>
        /// List of chat completion choices. Multiple choices if n > 1.
        /// </summary>
        public ChatCompletionResponseChoice[] choices { get; set; } = Array.Empty<ChatCompletionResponseChoice>();
    }

    /// <summary>
    /// A single choice in the completion response
    /// </summary>
    public class ChatCompletionResponseChoice : BaseCompletionResponseChoice
    {
        /// <summary>
        /// Message generated by the model for the chat completion
        /// </summary>
        public ChatCompletionMessage message { get; set; } = new();
    }

    /// <summary>
    /// Chat Completion Response for Streaming
    /// Documentation: https://platform.openai.com/docs/api-reference/chat/streaming
    /// </summary>
    public class ChatCompletionChunkResponse : BaseCompletionResponse
    {
        /// <summary>
        /// Object type, always "chat.completion.chunk"
        /// </summary>
        public string @object = "chat.completion.chunk";

        /// <summary>
        /// List of chat completion choices. Multiple choices if n > 1.
        /// </summary>
        public ChatCompletionChunkResponseChoice[] choices { get; set; } = Array.Empty<ChatCompletionChunkResponseChoice>();
    }

    /// <summary>
    /// Details of a completion choice in a streaming response
    /// </summary>
    public class ChatCompletionChunkResponseChoice : BaseCompletionResponseChoice
    {
        /// <summary>
        /// Incremental chat completion generated by the streaming model response
        /// </summary>
        public ChatCompletionMessage? delta { get; set; } = new();
    }
}