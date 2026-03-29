using Microsoft.Extensions.Logging;

namespace CarPartBotApi.Infrastructure.Logging;

public static partial class ApplicationLogs
{
    // HTTP clients.
    [LoggerMessage(EventId = 2_001_001, Level = LogLevel.Information, Message = "Successful external HTTP call. " +
        "MethodName: {MethodName}. Status code: {StatusCode}.")]
    public static partial void SuccessfulHttpCall(this ILogger logger, string? methodName, int statusCode);

    [LoggerMessage(EventId = 2_001_002, Level = LogLevel.Warning, Message = "Failed to read external HTTP response: response is empty. " +
        "MethodName: {MethodName}. Status code: {StatusCode}.")]
    public static partial void FailedToReadHttpResponse(this ILogger logger, string? methodName, int statusCode);

    [LoggerMessage(EventId = 2_001_003, Level = LogLevel.Warning, Message = "Failed to read external HTTP response. " +
        "MethodName: {MethodName}. Status code: {StatusCode}.")]
    public static partial void FailedToReadHttpResponse(this ILogger logger, string? methodName, int statusCode, Exception exception);

    [LoggerMessage(EventId = 2_001_004, Level = LogLevel.Warning,
        Message = "External HTTP call failed but was handled by the client. " +
        "MethodName: {MethodName}. Status code: {StatusCode}. Error message: {ErrorMessage}.")]
    public static partial void FailedHttpCall(this ILogger logger, string? methodName, int statusCode, string errorMessage);

    [LoggerMessage(EventId = 2_001_005, Level = LogLevel.Warning, Message = "External HTTP call failed and can't be handled by the client: response is empty. " +
        "MethodName: {MethodName}. Status code: {StatusCode}. Raw response: {RawResponse}.")]
    public static partial void FailedToReadHttpErrorResponse(this ILogger logger, string? methodName, int statusCode, string? rawResponse);

    [LoggerMessage(EventId = 2_001_006, Level = LogLevel.Warning, Message = "External HTTP call failed and can't be handled by the client. " +
        "MethodName: {MethodName}. Status code: {StatusCode}. Raw response: {RawResponse}.")]
    public static partial void FailedToReadHttpErrorResponse(this ILogger logger, string? methodName, int statusCode, string? rawResponse, Exception exception);
}
