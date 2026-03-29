namespace CarPartBotApi.Infrastructure.Clients.Telegram.Contracts;

internal sealed record RegisterWebhookRequest
{
    public required string Url { get; init; }
}
