using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Domain.Interfaces.Clients;
using CarPartBotApi.Infrastructure.Clients.Abstractions;
using CarPartBotApi.Infrastructure.Clients.Telegram.Contracts;
using CarPartBotApi.Infrastructure.Constants.Telegram;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utilities;

namespace CarPartBotApi.Infrastructure.Clients.Telegram;

public sealed class TelegramClient(
    HttpClient _httpClient,
    IOptionsSnapshot<InfrastructureSettings> _infrastructureOptions,
    IOptionsSnapshot<TelegramSettings> _telegramOptions,
    ILogger<TelegramClient> _logger)
    : BaseClient(_httpClient, _logger), ITelegramClient
{
    private const string WebhookRegistrationEndpointUrl = "setWebhook";

    protected override JsonSerializerOptions SerializerOptions => new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<Result> RegisterWebhook(CancellationToken ct)
    {
        if (_infrastructureOptions.Value.WebhookEndpointUrl is null)
        {
            return Result.Fail("Webhook URL is not configured.");
        }

        var webhookUrl = $"{_infrastructureOptions.Value.ApiBaseUrl}/{_infrastructureOptions.Value.WebhookEndpointUrl}";

        var request = new RegisterWebhookRequest
        {
            Url = webhookUrl,
            AllowedUpdates = [AllowedUpdateTypes.Message],
            SecretToken = _telegramOptions.Value.Webhook.SecretToken,
            MaxConnections = _telegramOptions.Value.Webhook.MaxConnections
        };

        using var response = await HttpClient.PostAsJsonAsync(WebhookRegistrationEndpointUrl, request, SerializerOptions, ct);

        if (response.IsSuccessStatusCode)
        {
            return Result.Succeed();
        }

        return await HandleError<RegisterWebhookErrorResponse>(response, ct);
    }
}
