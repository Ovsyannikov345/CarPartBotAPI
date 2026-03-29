using Microsoft.Extensions.Logging;

namespace CarPartBotApi.Application.Logging;

public static partial class ApplicationLogs
{
    // Telegram webhook registration.

    [LoggerMessage(EventId = 1_001_001, Level = LogLevel.Information, Message = "Telegram webhook registered successfully.")]
    public static partial void TelegramWebhookRegistered(this ILogger logger);

    [LoggerMessage(EventId = 1_001_002, Level = LogLevel.Error, Message = "Failed to register telegram webhook. Error message: {ErrorMessage}.")]
    public static partial void FailedToRegisterTelegramWebhook(this ILogger logger, string errorMessage);

    [LoggerMessage(EventId = 1_001_003, Level = LogLevel.Error, Message = "Failed to register telegram webhook.")]
    public static partial void FailedToRegisterTelegramWebhook(this ILogger logger, Exception exception);
}
