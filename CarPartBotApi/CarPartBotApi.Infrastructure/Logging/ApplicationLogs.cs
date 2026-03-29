using Microsoft.Extensions.Logging;

namespace CarPartBotApi.Infrastructure.Logging;

public static partial class ApplicationLogs
{
    // HTTP clients.

    [LoggerMessage(EventId = 2_001_001, Level = LogLevel.Information, Message = "Successful external HTTP call. MethodName: {MethodName}.")]
    public static partial void SuccessfulHttpCall(this ILogger logger, string? methodName);

    [LoggerMessage(EventId = 2_001_002, Level = LogLevel.Warning, Message = "Failed to read external HTTP response: response is empty. MethodName: {MethodName}.")]
    public static partial void FailedToReadHttpResponse(this ILogger logger, string? methodName);

    [LoggerMessage(EventId = 2_001_003, Level = LogLevel.Warning, Message = "Failed to read external HTTP response. MethodName: {MethodName}.")]
    public static partial void FailedToReadHttpResponse(this ILogger logger, string? methodName, Exception exception);

    [LoggerMessage(EventId = 2_001_004, Level = LogLevel.Warning, 
        Message = "External HTTP call failed but was handled by the client. MethodName: {MethodName}. Error message: {ErrorMessage}.")]
    public static partial void FailedHttpCall(this ILogger logger, string? methodName, string errorMessage);

    [LoggerMessage(EventId = 2_001_005, Level = LogLevel.Warning, 
        Message = "External HTTP call failed and can't be handled by the client: response is empty. MethodName: {MethodName}. Raw response: {RawResponse}.")]
    public static partial void FailedToReadHttpErrorResponse(this ILogger logger, string? methodName, string? rawResponse);

    [LoggerMessage(EventId = 2_001_006, Level = LogLevel.Warning, 
        Message = "External HTTP call failed and can't be handled by the client. MethodName: {MethodName}. Raw response: {RawResponse}.")]
    public static partial void FailedToReadHttpErrorResponse(this ILogger logger, string? methodName, string? rawResponse, Exception exception);
}
