using Lisa.AI.Blazor.Config;
using Lisa.AI.Blazor.Cpu.Transform;
using Lisa.AI.Blazor.FunctionCall;
using Lisa.AI.OpenAIModels;
using LLama;
using LLama.Common;
using LLama.Sampling;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Lisa.AI.Blazor.Cpu.Services
{
    /// <summary>
    /// LLM Model Service
    /// </summary>
    public class LLmModelService : ILLmModelService
    {
        private readonly ILogger<LLmModelService> _logger;
        private readonly List<LLmModelSettings> _settings;
        private readonly ToolPromptGenerator _toolPromptGenerator;
        private LLmModelSettings _currentSettings;
        private LLamaWeights _model;
        private LLamaEmbedder? _embedder;

        private int _loadedModelIndex = -1;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        /// <summary>
        /// Initializes the model based on the current index.
        /// </summary>
        public void InitModelIndex()
        {
            if (_settings.Count == 0)
            {
                _logger.LogError("No model settings found.");
                throw new ArgumentException("No model settings.");
            }

            int loadModelIndex = GlobalSettings.CurrentModelIndex;

            if (GlobalSettings.IsModelLoaded && _loadedModelIndex == loadModelIndex)
            {
                _logger.LogInformation("Model is already loaded.");
                return;
            }

            if (loadModelIndex < 0 || loadModelIndex >= _settings.Count)
            {
                _logger.LogError("Invalid model index: {modelIndex}.", loadModelIndex);
                throw new ArgumentException("Invalid model index.");
            }

            var settings = _settings[loadModelIndex];

            if (string.IsNullOrWhiteSpace(settings.ModelParams.ModelPath) || !File.Exists(settings.ModelParams.ModelPath))
            {
                _logger.LogError("Invalid model path: {path}.", settings.ModelParams.ModelPath);
                throw new ArgumentException("Invalid model path.");
            }

            DisposeModel();

            _model = LLamaWeights.LoadFromFile(settings.ModelParams);
            if (settings.ModelParams.Embeddings)
            {
                _embedder = new LLamaEmbedder(_model, settings.ModelParams);
            }

            _currentSettings = settings;
            _loadedModelIndex = loadModelIndex;
            GlobalSettings.IsModelLoaded = true;
        }

        /// <summary>
        /// Constructor for LLmModelService.
        /// </summary>
        public LLmModelService(IOptions<List<LLmModelSettings>> options, ILogger<LLmModelService> logger, ToolPromptGenerator toolPromptGenerator)
        {
            _logger = logger;
            _settings = options.Value;
            _toolPromptGenerator = toolPromptGenerator;
            InitModelIndex();
            if (_currentSettings == null || _model == null)
            {
                throw new InvalidOperationException("Failed to initialize the model.");
            }
        }

        /// <summary>
        /// Gets model metadata information.
        /// </summary>
        public IReadOnlyDictionary<string, string> GetModelInfo()
        {
            return _model.Metadata;
        }

        #region ChatCompletion

        /// <summary>
        /// Chat completion
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ChatCompletionResponse> CreateChatCompletionAsync(ChatCompletionRequest request, CancellationToken cancellationToken)
        {
            // No messages
            if (request.messages is null || request.messages.Length == 0)
            {
                _logger.LogWarning("No message in chat history.");
                return new ChatCompletionResponse();
            }

            var chatHistory = GetChatHistory(request);
            var genParams = GetInferenceParams(request, chatHistory.ToolStopWords);
            var ex = new MyStatelessExecutor(_model, _currentSettings.ModelParams);
            var result = new StringBuilder();

            var messagesContent = request.messages.Select(x => x.content).ToArray();
            var prompt_context = string.Join("", messagesContent);
            var completion_tokens = 0;

            _logger.LogDebug("Prompt context: {prompt_context}", chatHistory.ChatHistory);

            await foreach (var output in ex.InferAsync(chatHistory.ChatHistory, genParams, cancellationToken))
            {
                _logger.LogTrace("Message: {output}", output);
                result.Append(output);
                completion_tokens++;
            }
            var prompt_tokens = ex.PromptTokens;

            _logger.LogDebug("Prompt tokens: {prompt_tokens}, Completion tokens: {completion_tokens}", prompt_tokens, completion_tokens);
            _logger.LogDebug("Completion result: {result}", result);

            // Check tool return
            if (chatHistory.IsToolPromptEnabled)
            {
                var tools = _toolPromptGenerator.GenerateToolCall(result.ToString(), _currentSettings.ToolPrompt.Index);
                if (tools.Count > 0)
                {
                    _logger.LogDebug("Tool calls: {tools}", tools.Select(x => x.name));
                    return new ChatCompletionResponse
                    {
                        id = $"chatcmpl-{Guid.NewGuid():N}",
                        model = request.model,
                        created = DateTimeOffset.Now.ToUnixTimeSeconds(),
                        choices = new[]
                        {
                    new ChatCompletionResponseChoice
                    {
                        index = 0,
                        finish_reason = "tool_calls",
                        message = new ChatCompletionMessage
                        {
                            role = "assistant",
                            tool_calls = tools.Select(x => new ToolMessage
                            {
                                id = $"call_{Guid.NewGuid():N}",
                                function = new ToolMessageFuntion
                                {
                                    name = x.name,
                                    arguments = x.arguments
                                }
                            }).ToArray()
                        }
                    }
                },
                        usage = new UsageInfo
                        {
                            prompt_tokens = prompt_tokens,
                            completion_tokens = completion_tokens,
                            total_tokens = prompt_tokens + completion_tokens
                        }
                    };
                }
            }

            return new ChatCompletionResponse
            {
                id = $"chatcmpl-{Guid.NewGuid():N}",
                model = request.model,
                created = DateTimeOffset.Now.ToUnixTimeSeconds(),
                choices = new[]
                {
            new ChatCompletionResponseChoice
            {
                index = 0,
                finish_reason = completion_tokens >= request.max_tokens ? "length" : "stop",
                message = new ChatCompletionMessage
                {
                    role = "assistant",
                    content = result.ToString()
                }
            }
        },
                usage = new UsageInfo
                {
                    prompt_tokens = prompt_tokens,
                    completion_tokens = completion_tokens,
                    total_tokens = prompt_tokens + completion_tokens
                }
            };
        }

        /// <summary>
        /// Streamed chat completion
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<string> CreateChatCompletionStreamAsync(ChatCompletionRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // No messages
            if (request.messages is null || request.messages.Length == 0)
            {
                _logger.LogWarning("No message in chat history.");
                yield break;
            }

            var chatHistory = GetChatHistory(request);
            var genParams = GetInferenceParams(request, chatHistory.ToolStopWords);
            var ex = new MyStatelessExecutor(_model, _currentSettings.ModelParams);

            var id = $"chatcmpl-{Guid.NewGuid():N}";
            var created = DateTimeOffset.Now.ToUnixTimeSeconds();

            int index = 0;

            _logger.LogDebug("Prompt context: {prompt_context}", chatHistory.ChatHistory);

            var chunk = JsonSerializer.Serialize(new ChatCompletionChunkResponse
            {
                id = id,
                created = created,
                model = request.model,
                choices = new[]
                {
            new ChatCompletionChunkResponseChoice
            {
                index = index,
                delta = new ChatCompletionMessage
                {
                    role = "assistant"
                },
                finish_reason = null
            }
        }
            }, _jsonSerializerOptions);
            yield return $"data: {chunk}\n\n";

            // Token interception for tool activation
            List<string> tokens = new();
            bool toolActive = false;

            await foreach (var output in ex.InferAsync(chatHistory.ChatHistory, genParams, cancellationToken))
            {
                _logger.LogTrace("Message: {output}", output);

                if (chatHistory.IsToolPromptEnabled)
                {
                    if (toolActive || tokens.Count < 3)
                    {
                        tokens.Add(output);
                        continue;
                    }

                    toolActive = _toolPromptGenerator.IsToolActive(tokens, _currentSettings.ToolPrompt.Index);
                    if (toolActive)
                    {
                        _logger.LogDebug("Tool is active.");
                        tokens.Add(output);
                        continue;
                    }
                    else
                    {
                        chatHistory.IsToolPromptEnabled = false;
                        foreach (var token in tokens)
                        {
                            chunk = JsonSerializer.Serialize(new ChatCompletionChunkResponse
                            {
                                id = id,
                                created = created,
                                model = request.model,
                                choices = new[]
                                {
                            new ChatCompletionChunkResponseChoice
                            {
                                index = ++index,
                                delta = new ChatCompletionMessage
                                {
                                    role = null,
                                    content = token
                                },
                                finish_reason = null
                            }
                        }
                            }, _jsonSerializerOptions);
                            yield return $"data: {chunk}\n\n";
                        }
                    }
                }

                chunk = JsonSerializer.Serialize(new ChatCompletionChunkResponse
                {
                    id = id,
                    created = created,
                    model = request.model,
                    choices = new[]
                    {
                new ChatCompletionChunkResponseChoice
                {
                    index = ++index,
                    delta = new ChatCompletionMessage
                    {
                        role = null,
                        content = output
                    },
                    finish_reason = null
                }
            }
                }, _jsonSerializerOptions);
                yield return $"data: {chunk}\n\n";
            }

            if (toolActive)
            {
                var result = string.Join("", tokens).Trim();
                var tools = _toolPromptGenerator.GenerateToolCall(result, _currentSettings.ToolPrompt.Index);
                if (tools.Count > 0)
                {
                    _logger.LogDebug("Tool calls: {tools}", tools.Select(x => x.name));
                    chunk = JsonSerializer.Serialize(new ChatCompletionChunkResponse
                    {
                        id = id,
                        created = created,
                        model = request.model,
                        choices = new[]
                        {
                    new ChatCompletionChunkResponseChoice
                    {
                        index = ++index,
                        delta = new ChatCompletionMessage
                        {
                            role = null,
                            tool_calls = tools.Select(x => new ToolMessage
                            {
                                id = $"call_{Guid.NewGuid():N}",
                                function = new ToolMessageFuntion
                                {
                                    name = x.name,
                                    arguments = x.arguments
                                }
                            }).ToArray()
                        },
                        finish_reason = "tool_calls"
                    }
                }
                    }, _jsonSerializerOptions);
                    yield return $"data: {chunk}\n\n";
                }
                else
                {
                    foreach (var token in tokens)
                    {
                        chunk = JsonSerializer.Serialize(new ChatCompletionChunkResponse
                        {
                            id = id,
                            created = created,
                            model = request.model,
                            choices = new[]
                            {
                        new ChatCompletionChunkResponseChoice
                        {
                            index = ++index,
                            delta = new ChatCompletionMessage
                            {
                                role = null,
                                content = token
                            },
                            finish_reason = null
                        }
                    }
                        }, _jsonSerializerOptions);
                        yield return $"data: {chunk}\n\n";
                    }
                }

                chunk = JsonSerializer.Serialize(new ChatCompletionChunkResponse
                {
                    id = id,
                    created = created,
                    model = request.model,
                    choices = new[]
                    {
                new ChatCompletionChunkResponseChoice
                {
                    index = tools.Count > 0 ? 0 : ++index,
                    delta = null,
                    finish_reason = tools.Count > 0 ? "tool_calls" : "stop"
                }
            }
                }, _jsonSerializerOptions);
                yield return $"data: {chunk}\n\n";
                yield return "data: [DONE]\n\n";
                yield break;
            }

            chunk = JsonSerializer.Serialize(new ChatCompletionChunkResponse
            {
                id = id,
                created = created,
                model = request.model,
                choices = new[]
                {
            new ChatCompletionChunkResponseChoice
            {
                index = ++index,
                delta = null,
                finish_reason = "stop"
            }
        }
            }, _jsonSerializerOptions);
            yield return $"data: {chunk}\n\n";
            yield return "data: [DONE]\n\n";
            yield break;
        }

        /// <summary>
        /// Generates chat history
        /// </summary>
        /// <param name="request">Request details</param>
        /// <returns></returns>
        private ChatHistoryResult GetChatHistory(ChatCompletionRequest request)
        {
            var toolPrompt = _toolPromptGenerator.GenerateToolPrompt(request, _currentSettings.ToolPrompt.Index, _currentSettings.ToolPrompt.Lang);
            var toolEnabled = !string.IsNullOrWhiteSpace(toolPrompt);
            var toolStopWords = toolEnabled ? _toolPromptGenerator.GetToolStopWords(_currentSettings.ToolPrompt.Index) : null;

            var messages = request.messages;

            if ((toolEnabled || !string.IsNullOrWhiteSpace(_currentSettings.SystemPrompt)) && messages.First()?.role != "system")
            {
                _logger.LogDebug("Add system prompt.");
                messages = messages.Prepend(new ChatCompletionMessage
                {
                    role = "system",
                    content = _currentSettings.SystemPrompt
                }).ToArray();
            }

            string history = "";
            if (_currentSettings.WithTransform?.HistoryTransform != null)
            {
                var type = Type.GetType(_currentSettings.WithTransform.HistoryTransform);
                if (type != null)
                {
                    var historyTransform = Activator.CreateInstance(type) as ITemplateTransform;
                    if (historyTransform != null)
                    {
                        history = historyTransform.HistoryToText(messages, _toolPromptGenerator, _currentSettings.ToolPrompt, toolPrompt);
                    }
                }
            }
            else
            {
                history = new BaseHistoryTransform().HistoryToText(messages, _toolPromptGenerator, _currentSettings.ToolPrompt, toolPrompt);
            }

            return new ChatHistoryResult(history, toolEnabled, toolStopWords);
        }

        #endregion

        #region Embedding

        /// <summary>
        /// Create embedding
        /// </summary>
        /// <param name="request">Request content</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Word embeddings</returns>
        public async Task<EmbeddingResponse> CreateEmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken)
        {
            var embeddings = new List<EmbeddingObject>();

            if (request.input is null || request.input.Length == 0)
            {
                _logger.LogWarning("No input.");
                return new EmbeddingResponse();
            }

            if (!_currentSettings.ModelParams.Embeddings)
            {
                _logger.LogWarning("Model does not support embeddings.");
                return new EmbeddingResponse();
            }

            if (_embedder == null)
            {
                _logger.LogWarning("Embedder is null.");
                return new EmbeddingResponse();
            }

            int index = 0;
            foreach (var text in request.input)
            {
                embeddings.Add(new EmbeddingObject
                {
                    embedding = await _embedder.GetEmbeddings(text, cancellationToken),
                    index = index++
                });
            }

            return new EmbeddingResponse
            {
                data = embeddings.ToArray(),
                model = request.model
            };
        }

        /// <summary>
        /// Indicates whether embeddings are supported
        /// </summary>
        public bool IsSupportEmbedding => _currentSettings.ModelParams.Embeddings;

        #endregion
        #region Completion

        /// <summary>
        /// Completion generation
        /// </summary>
        public async Task<CompletionResponse> CreateCompletionAsync(CompletionRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.prompt))
            {
                _logger.LogWarning("No prompt.");
                return new CompletionResponse();
            }
            var genParams = GetInferenceParams(request, null);
            var ex = new MyStatelessExecutor(_model, _currentSettings.ModelParams);
            var result = new StringBuilder();

            var completion_tokens = 0;
            await foreach (var output in ex.InferAsync(request.prompt, genParams, cancellationToken))
            {
                _logger.LogDebug("Message: {output}", output);
                result.Append(output);
                completion_tokens++;
            }
            var prompt_tokens = ex.PromptTokens;

            return new CompletionResponse
            {
                id = $"cmpl-{Guid.NewGuid():N}",
                model = request.model,
                created = DateTimeOffset.Now.ToUnixTimeSeconds(),
                choices = new[]
                {
            new CompletionResponseChoice
            {
                index = 0,
                text = result.ToString()
            }
        },
                usage = new UsageInfo
                {
                    prompt_tokens = prompt_tokens,
                    completion_tokens = completion_tokens,
                    total_tokens = prompt_tokens + completion_tokens
                }
            };
        }

        /// <summary>
        /// Streamed completion generation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<string> CreateCompletionStreamAsync(CompletionRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.prompt))
            {
                _logger.LogWarning("No prompt.");
                yield break;
            }
            var genParams = GetInferenceParams(request, null);
            var ex = new MyStatelessExecutor(_model, _currentSettings.ModelParams);
            var id = $"cmpl-{Guid.NewGuid():N}";
            var created = DateTimeOffset.Now.ToUnixTimeSeconds();
            int index = 0;
            var chunk = JsonSerializer.Serialize(new CompletionResponse
            {
                id = id,
                created = created,
                model = request.model,
                choices = new[]
                {
            new CompletionResponseChoice
            {
                index = index,
                text = "",
                finish_reason = null
            }
        }
            }, _jsonSerializerOptions);
            yield return $"data: {chunk}\n\n";
            await foreach (var output in ex.InferAsync(request.prompt, genParams, cancellationToken))
            {
                _logger.LogDebug("Message: {output}", output);
                chunk = JsonSerializer.Serialize(new CompletionResponse
                {
                    id = id,
                    created = created,
                    model = request.model,
                    choices = new[]
                    {
                new CompletionResponseChoice
                {
                    index = ++index,
                    text = output,
                    finish_reason = null
                }
            }
                }, _jsonSerializerOptions);
                yield return $"data: {chunk}\n\n";
            }

            chunk = JsonSerializer.Serialize(new CompletionResponse
            {
                id = id,
                created = created,
                model = request.model,
                choices = new[]
                {
            new CompletionResponseChoice
            {
                index = ++index,
                text = null,
                finish_reason = "stop"
            }
        }
            }, _jsonSerializerOptions);
            yield return $"data: {chunk}\n\n";
            yield return "data: [DONE]\n\n";
            yield break;
        }

        #endregion

        /// <summary>
        /// Generate inference parameters
        /// </summary>
        /// <param name="request">The request object</param>
        /// <param name="toolstopwords">Tool-specific stop words</param>
        /// <returns>Inference parameters</returns>
        private InferenceParams GetInferenceParams(BaseCompletionRequest request, string[]? toolstopwords)
        {
            var stop = new List<string>();
            if (request.stop != null)
            {
                stop.AddRange(request.stop);
            }
            if (_currentSettings.AntiPrompts?.Length > 0)
            {
                stop.AddRange(_currentSettings.AntiPrompts);
            }
            if (toolstopwords?.Length > 0)
            {
                stop.AddRange(toolstopwords);
            }
            if (stop.Count > 0)
            {
                // Remove duplicates, trim empty entries, and limit to a maximum of 4 entries
                stop = stop.Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).Take(4).ToList();
            }

            InferenceParams inferenceParams = new InferenceParams()
            {
                MaxTokens = request.max_tokens.HasValue && request.max_tokens.Value > 0 ? request.max_tokens.Value : -1,
                AntiPrompts = stop,
                SamplingPipeline = new DefaultSamplingPipeline
                {
                    Temperature = request.temperature,
                    TopP = request.top_p,
                    AlphaPresence = request.presence_penalty,
                    AlphaFrequency = request.frequency_penalty,
                    // Deterministic sampling settings
                    Seed = request.seed is null ? (uint)Random.Shared.Next() : request.seed.Value
                }
            };
            return inferenceParams;
        }

        #region Dispose

        /// <summary>
        /// Manually release model resources
        /// </summary>
        public void DisposeModel()
        {
            if (GlobalSettings.IsModelLoaded)
            {
                _embedder?.Dispose();
                _model.Dispose();
                GlobalSettings.IsModelLoaded = false;
                _loadedModelIndex = -1;
            }
        }

        /// <summary>
        /// Indicates whether resources have been disposed
        /// </summary>
        private bool _disposedValue = false;

        /// <summary>
        /// Release unmanaged resources
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    DisposeModel();
                }
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
