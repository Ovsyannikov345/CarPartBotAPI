using CarPartBotApi.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Utilities;

namespace CarPartBotApi.Infrastructure.Clients.Abstractions;

public abstract class BaseClient(HttpClient _httpClient, ILogger _logger)
{
    protected abstract JsonSerializerOptions SerializerOptions { get; }

    protected readonly HttpClient HttpClient = _httpClient;

    protected readonly ILogger Logger = _logger;

    protected virtual async Task<Result<T>> HandleResponse<T>(
        HttpResponseMessage message,
        CancellationToken ct,
        [CallerMemberName] string? callerMethodName = null)
    {
        try
        {
            var response = await message.Content.ReadFromJsonAsync<T>(SerializerOptions, ct);

            if (response is not null)
            {
                Logger.SuccessfulHttpCall(callerMethodName, (int)message.StatusCode);

                return response;
            }

            Logger.FailedToReadHttpResponse(callerMethodName, (int)message.StatusCode);

            return Result<T>.Fail("Failed to read API response.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Logger.FailedToReadHttpResponse(callerMethodName, (int)message.StatusCode, ex);

            return Result<T>.Fail("Failed to read API response.");
        }
    }

    protected virtual async Task<Result> HandleError<T>(
        HttpResponseMessage message,
        CancellationToken ct,
        [CallerMemberName] string? callerMethodName = null) where T : ErrorResponse
    {
        string? rawResponse = null;

        try
        {
            rawResponse = await message.Content.ReadAsStringAsync(ct);

            var response = JsonSerializer.Deserialize<T>(rawResponse, SerializerOptions);

            if (response is not null)
            {
                Logger.FailedHttpCall(callerMethodName, (int)message.StatusCode, response.GetErrorMessage());

                return Result.Fail(response.GetErrorMessage());
            }

            Logger.FailedToReadHttpErrorResponse(callerMethodName, (int)message.StatusCode, rawResponse);

            return Result<T>.Fail("Failed to read API response.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Logger.FailedToReadHttpErrorResponse(callerMethodName, (int)message.StatusCode, rawResponse, ex);

            return Result<T>.Fail("Failed to read API response.");
        }
    }
}
