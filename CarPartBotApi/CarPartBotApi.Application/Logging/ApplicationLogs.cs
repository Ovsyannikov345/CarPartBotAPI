using CarPartBotApi.Application.Constants.Enums;
using Microsoft.Extensions.Logging;

namespace CarPartBotApi.Application.Logging;

public static partial class ApplicationLogs
{
    #region Telegram webhook registration.

    [LoggerMessage(EventId = 1_001_001, Level = LogLevel.Information, Message = "Telegram webhook registered successfully.")]
    public static partial void TelegramWebhookRegistered(this ILogger logger);

    [LoggerMessage(EventId = 1_001_002, Level = LogLevel.Error, Message = "Failed to register telegram webhook. Reason: {Reason}.")]
    public static partial void FailedToRegisterTelegramWebhook(this ILogger logger, string reason);

    [LoggerMessage(EventId = 1_001_003, Level = LogLevel.Error, Message = "Failed to register telegram webhook.")]
    public static partial void FailedToRegisterTelegramWebhook(this ILogger logger, Exception exception);

    #endregion

    #region Telegram webhook background processing

    [LoggerMessage(EventId = 1_002_001, Level = LogLevel.Information, Message = "Enqueued telegram webhook notification for background processing. Raw payload: {RawPayload}.")]
    public static partial void EnqueuedTelegramWebhookNotification(this ILogger logger, string rawPayload);

    [LoggerMessage(EventId = 1_002_002, Level = LogLevel.Information, Message = "Successfully processed telegram webhook notification.")]
    public static partial void ProcessedTelegramWebhookNotification(this ILogger logger);

    [LoggerMessage(EventId = 1_002_003, Level = LogLevel.Error, Message = "Failed to process telegram webhook notification. Reason: {Reason}.")]
    public static partial void FailedToProcessTelegramWebhookNotification(this ILogger logger, string reason);

    [LoggerMessage(EventId = 1_002_004, Level = LogLevel.Error, Message = "Failed to process telegram webhook notification.")]
    public static partial void FailedToProcessTelegramWebhookNotification(this ILogger logger, Exception exception);

    #endregion

    #region Telegram webhook handling

    [LoggerMessage(EventId = 1_003_001, Level = LogLevel.Error, Message = "Failed to create telegram webhook reader. Raw payload: {RawPayload}.")]
    public static partial void FailedToCreateWebhookReader(this ILogger logger, string rawPayload);

    [LoggerMessage(EventId = 1_003_002, Level = LogLevel.Error, Message = "Failed to handle telegram webhook event. Reason: {Reason}")]
    public static partial void FailedToHandleTelegramWebhookEvent(this ILogger logger, string reason);

    [LoggerMessage(EventId = 1_003_003, Level = LogLevel.Information, Message = "Successfully handled telegram webhook event.")]
    public static partial void HandledTelegramWebhookEvent(this ILogger logger);

    #endregion

    #region Command execution pipeline

    [LoggerMessage(EventId = 1_004_001, Level = LogLevel.Error, Message = "Command execution pipeline has failed. Reason: {Reason}.")]
    public static partial void CommandExecutionPipelineFailed(this ILogger logger, string reason);

    [LoggerMessage(EventId = 1_004_002, Level = LogLevel.Error, Message = "Command execution pipeline has failed.")]
    public static partial void CommandExecutionPipelineFailed(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 1_004_003, Level = LogLevel.Information, Message = "Command execution pipeline has succeeded.")]
    public static partial void CommandExecutionPipelineSucceeded(this ILogger logger);

    #endregion

    #region Command/failure handlers

    [LoggerMessage(EventId = 1_005_001, Level = LogLevel.Error, Message = "Failed to gracefully handle failure. Reason: {Reason}.")]
    public static partial void FailedToGracefullyHandleFailure(this ILogger logger, string reason);

    [LoggerMessage(EventId = 1_005_002, Level = LogLevel.Error, Message = "Failed to gracefully handle failure.")]
    public static partial void FailedToGracefullyHandleFailure(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 1_005_002, Level = LogLevel.Information, Message = "Failure handled gracefully. Failure type: {FailureType}.")]
    public static partial void FailureHandledGracefully(this ILogger logger, HandlingFailureType failureType);

    #endregion
}
