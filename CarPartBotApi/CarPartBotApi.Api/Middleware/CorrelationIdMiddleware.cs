using CarPartBotApi.Api.Constants;
using CarPartBotApi.Application.Accessors;
using Serilog.Context;

namespace CarPartBotApi.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICorrelationIdAccessor correlationIdAccessor)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        correlationIdAccessor.CorrelationId = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers.TryAdd(HeaderNames.CorrelationIdHeader, correlationId);

            return Task.CompletedTask;
        });

        context.Items[HeaderNames.CorrelationIdHeader] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationIdAccessor.CorrelationId))
        {
            await next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderNames.CorrelationIdHeader, out var existingId) && !string.IsNullOrWhiteSpace(existingId))
        {
            return existingId.ToString();
        }

        return Guid.NewGuid().ToString("N");
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
