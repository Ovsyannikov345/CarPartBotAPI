namespace CarPartBotApi.Infrastructure.Clients.Telegram.Contracts;

internal sealed record RegisterWebhookRequest
{
    public required string Url { get; init; }

    public int? MaxConnections { get; init; }

    public required List<string>? AllowedUpdates { get; init; }

    public string? SecretToken { get; init; }
}
