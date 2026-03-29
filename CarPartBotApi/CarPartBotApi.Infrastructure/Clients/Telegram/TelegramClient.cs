using CarPartBotApi.Domain.Interfaces.Clients;
using CarPartBotApi.Infrastructure.Clients.Abstractions;
using CarPartBotApi.Infrastructure.Clients.Telegram.Contracts;
using CarPartBotApi.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utilities;

namespace CarPartBotApi.Infrastructure.Clients.Telegram;

public sealed class TelegramClient(
    HttpClient _httpClient,
    IOptionsSnapshot<InfrastructureSettings> _options,
    ILogger<TelegramClient> _logger)
    : BaseClient(_logger), ITelegramClient
{
    private const string WebhookRegistrationEndpointUrl = "setWebhook";

    protected override JsonSerializerOptions SerializerOptions => new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<Result> RegisterWebhook(CancellationToken ct)
    {
        var webhookUrl = $"{_options.Value.ApiBaseUrl}/{_options.Value.WebhookEndpointUrl}";

        var request = new RegisterWebhookRequest { Url = webhookUrl };

        using var response = await _httpClient.PostAsJsonAsync(WebhookRegistrationEndpointUrl, request, SerializerOptions, ct);

        if (response.IsSuccessStatusCode)
        {
            return Result.Succeed();
        }

        return await HandleError<RegisterWebhookErrorResponse>(response, ct);
    }
}
