namespace CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.Common;

internal sealed record MessageEntity
{
    public required string Type { get; init; }

    public int Offset { get; init; }

    public int Length { get; init; }
}
