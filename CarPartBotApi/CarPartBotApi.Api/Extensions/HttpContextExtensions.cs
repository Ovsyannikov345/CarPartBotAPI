namespace CarPartBotApi.Api.Extensions;

public static class HttpContextExtensions
{
    public static async Task<string> ReadRequestBodyAsStringAsync(this HttpContext context, CancellationToken ct)
    {
        context.Request.EnableBuffering();

        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);

        var body = await reader.ReadToEndAsync(ct);

        context.Request.Body.Position = 0;

        return body;
    }

    public static string? GetHeaderValueOrNull(this HttpContext context, string headerName)
    {
        if (context.Request.Headers.TryGetValue(headerName, out var headerValues))
        {
            return headerValues.FirstOrDefault();
        }

        return null;
    }
}
