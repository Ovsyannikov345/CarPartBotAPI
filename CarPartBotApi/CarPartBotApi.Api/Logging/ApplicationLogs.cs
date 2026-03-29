namespace CarPartBotApi.Api.Logging;

public static partial class ApplicationLogs
{
    // Webhook endpoints.

    [LoggerMessage(EventId = 3_001_001, Level = LogLevel.Warning, Message = "Webhook secret token is missing.")]
    public static partial void WebhookSecretTokenIsMissing(this ILogger logger);

    [LoggerMessage(EventId = 3_001_002, Level = LogLevel.Warning, Message = "Received webhook secret token is invalid.")]
    public static partial void WebhookSecretTokenIsInvalid(this ILogger logger);
}
