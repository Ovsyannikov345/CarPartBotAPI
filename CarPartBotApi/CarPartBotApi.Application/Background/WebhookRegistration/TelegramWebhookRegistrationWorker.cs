using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Application.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace CarPartBotApi.Application.Background.WebhookRegistration;

internal class TelegramWebhookRegistrationWorker(
    IServiceScopeFactory _serviceScopeFactory,
    IOptionsMonitor<TelegramSettings> _options,
    ILogger<TelegramWebhookRegistrationWorker> _logger) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentPeriodInMinutes = _options.CurrentValue.Webhook.RegistrationPeriodInMinutes;

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(currentPeriodInMinutes));

        while (!stoppingToken.IsCancellationRequested)
        {
            using var correlationContext = LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString("N"));

            try
            {
                await RegisterTelegramWebhook(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.FailedToRegisterTelegramWebhook(ex);
            }

            var newPeriodInMinutes = _options.CurrentValue.Webhook.RegistrationPeriodInMinutes;

            if (newPeriodInMinutes != currentPeriodInMinutes)
            {
                currentPeriodInMinutes = newPeriodInMinutes;
                timer.Period = TimeSpan.FromMinutes(currentPeriodInMinutes);
            }

            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task RegisterTelegramWebhook(CancellationToken ct)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var telegramClient = scope.ServiceProvider.GetRequiredService<ITelegramClient>();

        var registrationResult = await telegramClient.RegisterWebhook(ct);

        if (registrationResult.IsSuccess)
        {
            _logger.TelegramWebhookRegistered();
        }
        else
        {
            _logger.FailedToRegisterTelegramWebhook(registrationResult.ErrorMessage);
        }
    }
}
