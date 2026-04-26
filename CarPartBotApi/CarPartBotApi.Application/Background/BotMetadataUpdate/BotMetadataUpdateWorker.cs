using CarPartBotApi.Application.Background.WebhookRegistration;
using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Clients.Telegram.Dto;
using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Application.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace CarPartBotApi.Application.Background.BotMetadataUpdate;

internal class BotMetadataUpdateWorker(
    IServiceScopeFactory _serviceScopeFactory,
    IOptionsMonitor<TelegramSettings> _options,
    ILogger<TelegramWebhookRegistrationWorker> _logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO make configurable.
        var currentPeriodInMinutes = 10;

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(currentPeriodInMinutes));

        while (!stoppingToken.IsCancellationRequested)
        {
            using var correlationContext = LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString("N"));

            try
            {
                await UpdateBotMetadata(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.FailedToUpdateTelegramBotMetadata(ex);
            }

            // TODO make configurable.
            var newPeriodInMinutes = 10;

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

    private async Task UpdateBotMetadata(CancellationToken ct)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var telegramClient = scope.ServiceProvider.GetRequiredService<ITelegramClient>();

        var publicCommands = scope.ServiceProvider
            .GetServices<ICommandHandler>()
            .Where(h => h.CommandAccessLevel is not CommandAccessLevel.AdminUserOnly && h.CommandName != CommandNames.Start)
            .Select(h => new BotCommand
            {
                Command = h.CommandName,
                Description = h.CommandDescription
            })
            .ToList();

        var updateResult = await telegramClient.UpdateBotCommandsList(publicCommands, ct);

        if (updateResult.IsSuccess)
        {
            _logger.TelegramBotMetadataUpdated();
        }
        else
        {
            _logger.FailedToUpdateTelegramBotMetadata(updateResult.ErrorMessage);
        }
    }
}
