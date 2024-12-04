﻿using Lisa.AI.Config;
using Lisa.AI.Config.ModelSettings;
using Lisa.AI.FunctionCall;
using Lisa.AI.OpenAIModels.ChatCompletionModels;
using Lisa.AI.OpenAIModels.CompletionModels;
using Lisa.AI.OpenAIModels.EmbeddingModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using Timer = System.Timers.Timer;

namespace Lisa.AI.Services;

/// <summary>
/// Decorator class for LLM Model Service
/// </summary>
public class LLmModelDecorator : ILLmModelService
{
    private readonly ILLmModelService _llmService;
    private readonly ILogger<LLmModelService> _logger;
    private readonly ToolPromptGenerator _toolPromptGenerator;

    ///<inheritdoc/>
    public bool IsSupportEmbedding => _llmService.IsSupportEmbedding;

    ///<inheritdoc/>
    public LLmModelDecorator(IOptions<List<LLmModelSettings>> options, ILogger<LLmModelService> logger, ToolPromptGenerator toolPromptGenerator)
    {
        _logger = logger;
        _toolPromptGenerator = toolPromptGenerator;
        _llmService = new LLmModelService(options, logger, toolPromptGenerator);

        // Auto-release timer
        if (GlobalSettings.AutoReleaseTime > 0)
        {
            _logger.LogInformation("Auto release time: {time} min.", GlobalSettings.AutoReleaseTime);
            _idleThreshold = TimeSpan.FromMinutes(GlobalSettings.AutoReleaseTime);
            _lastUsedTime = DateTime.Now;
            _idleTimer = new Timer(60000); // Check every minute
            _idleTimer.Elapsed += CheckIdle;
            _idleTimer.Start();
        }
    }

    ///<inheritdoc/>
    public async Task<ChatCompletionResponse> CreateChatCompletionAsync(ChatCompletionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            BeginUseModel();
            return await _llmService.CreateChatCompletionAsync(request, cancellationToken);
        }
        finally
        {
            EndUseModel();
            _lastUsedTime = DateTime.Now;
        }
    }

    ///<inheritdoc/>
    public async IAsyncEnumerable<string> CreateChatCompletionStreamAsync(ChatCompletionRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        try
        {
            BeginUseModel();
            await foreach (var item in _llmService.CreateChatCompletionStreamAsync(request, cancellationToken))
            {
                yield return item;
            }
        }
        finally
        {
            EndUseModel();
            _lastUsedTime = DateTime.Now;
        }
    }

    ///<inheritdoc/>
    public async Task<CompletionResponse> CreateCompletionAsync(CompletionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            BeginUseModel();
            return await _llmService.CreateCompletionAsync(request, cancellationToken);
        }
        finally
        {
            EndUseModel();
            _lastUsedTime = DateTime.Now;
        }
    }

    ///<inheritdoc/>
    public async IAsyncEnumerable<string> CreateCompletionStreamAsync(CompletionRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        try
        {
            BeginUseModel();
            await foreach (var item in _llmService.CreateCompletionStreamAsync(request, cancellationToken))
            {
                yield return item;
            }
        }
        finally
        {
            EndUseModel();
            _lastUsedTime = DateTime.Now;
        }
    }

    ///<inheritdoc/>
    public async Task<EmbeddingResponse> CreateEmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            BeginUseModel();
            return await _llmService.CreateEmbeddingAsync(request, cancellationToken);
        }
        finally
        {
            EndUseModel();
            _lastUsedTime = DateTime.Now;
        }
    }

    ///<inheritdoc/>
    public IReadOnlyDictionary<string, string> GetModelInfo()
    {
        try
        {
            BeginUseModel();
            return _llmService.GetModelInfo();
        }
        finally
        {
            EndUseModel();
            _lastUsedTime = DateTime.Now;
        }
    }

    ///<inheritdoc/>
    public void InitModelIndex()
    {
        if (GlobalSettings.IsModelLoaded && _modelUsageCount != 0)
        {
            _logger.LogWarning("Model is in use.");
            throw new InvalidOperationException("Model is in use.");
        }
        _llmService.InitModelIndex();
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        _llmService.Dispose();
    }

    ///<inheritdoc/>
    public void DisposeModel()
    {
        _llmService.DisposeModel();
    }

    // Resource release timer
    private Timer? _idleTimer;
    private DateTime _lastUsedTime;
    private readonly TimeSpan _idleThreshold;

    // Model usage count
    private int _modelUsageCount = 0;

    /// <summary>
    /// Begin model usage
    /// </summary>
    public void BeginUseModel()
    {
        // Initialize model if not loaded
        if (!GlobalSettings.IsModelLoaded)
        {
            _llmService.InitModelIndex();
        }
        Interlocked.Increment(ref _modelUsageCount);
    }

    /// <summary>
    /// End model usage
    /// </summary>
    public void EndUseModel()
    {
        Interlocked.Decrement(ref _modelUsageCount);
    }

    /// <summary>
    /// Automatic model release check
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckIdle(object? sender, object e)
    {
        if (DateTime.Now - _lastUsedTime > _idleThreshold && GlobalSettings.IsModelLoaded && _modelUsageCount == 0)
        {
            _logger.LogInformation("Auto release model.");
            DisposeModel();
        }
    }
}