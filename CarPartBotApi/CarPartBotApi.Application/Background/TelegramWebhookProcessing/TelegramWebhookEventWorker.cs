using CarPartBotApi.Application.Logging;
using CarPartBotApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace CarPartBotApi.Application.Background.TelegramWebhookProcessing;

public sealed class TelegramWebhookEventWorker(
    ITelegramWebhookEventReader _queueReader,
    IServiceProvider _serviceProvider,
    ILogger<TelegramWebhookEventWorker> _logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _queueReader.ReadAll(stoppingToken))
        {
            using (LogContext.PushProperty("CorrelationId", message.CorrelationId))
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                await ProcessMessage(scope, message, stoppingToken);
            }
        }
    }

    private async Task ProcessMessage(AsyncServiceScope scope, TelegramWebhookMessage message, CancellationToken ct)
    {
        try
        {
            var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramService>();

            var processingResult = await telegramService.ProcessTelegramEvent(message.RawPayload, ct);

            if (processingResult.IsSuccess)
            {
                _logger.ProcessedTelegramWebhookEvent();
            }
            else
            {
                _logger.FailedToProcessTelegramWebhookEvent(processingResult.ErrorMessage);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.FailedToProcessTelegramWebhookEvent(ex);
        }
    }
}
