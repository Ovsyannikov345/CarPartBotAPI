using CarPartBotApi.Api.Logging;
using CarPartBotApi.Api.Responses;
using System.Text.Json;

namespace CarPartBotApi.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> _logger)
{
    private static readonly JsonSerializerOptions ExceptionSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested == true)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 499;
            }

            _logger.RequestCancelled();
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse
            {
                ErrorMessage = "Unknown error occurred",
                StatusCode = 500
            }, ExceptionSerializerOptions));

            _logger.UnhandledException(ex);
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
