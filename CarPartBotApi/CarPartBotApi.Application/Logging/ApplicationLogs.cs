using Microsoft.Extensions.Logging;

namespace CarPartBotApi.Application.Logging;

public static partial class ApplicationLogs
{
    #region Telegram webhook registration.

    [LoggerMessage(EventId = 1_001_001, Level = LogLevel.Information, Message = "Telegram webhook registered successfully.")]
    public static partial void TelegramWebhookRegistered(this ILogger logger);

    [LoggerMessage(EventId = 1_001_002, Level = LogLevel.Error, Message = "Failed to register telegram webhook. Error message: {ErrorMessage}.")]
    public static partial void FailedToRegisterTelegramWebhook(this ILogger logger, string errorMessage);

    [LoggerMessage(EventId = 1_001_003, Level = LogLevel.Error, Message = "Failed to register telegram webhook.")]
    public static partial void FailedToRegisterTelegramWebhook(this ILogger logger, Exception exception);

    #endregion

    #region Telegram webhook background processing

    [LoggerMessage(EventId = 1_002_001, Level = LogLevel.Information, Message = "Enqueued telegram webhook event for background processing. Raw payload: {RawPayload}.")]
    public static partial void EnqueuedTelegramWebhookEvent(this ILogger logger, string rawPayload);

    [LoggerMessage(EventId = 1_002_002, Level = LogLevel.Information, Message = "Successfully processed telegram webhook event.")]
    public static partial void ProcessedTelegramWebhookEvent(this ILogger logger);

    [LoggerMessage(EventId = 1_002_003, Level = LogLevel.Warning, Message = "Failed to process telegram webhook event. Error message: {ErrorMessage}.")]
    public static partial void FailedToProcessTelegramWebhookEvent(this ILogger logger, string errorMessage);

    [LoggerMessage(EventId = 1_002_004, Level = LogLevel.Warning, Message = "Failed to process telegram webhook event.")]
    public static partial void FailedToProcessTelegramWebhookEvent(this ILogger logger, Exception exception);

    #endregion
}
