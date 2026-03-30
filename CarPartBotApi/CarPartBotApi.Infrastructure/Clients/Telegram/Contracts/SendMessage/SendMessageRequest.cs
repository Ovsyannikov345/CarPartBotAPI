using CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.Common;

namespace CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.SendMessage;

internal sealed record SendMessageRequest
{
    public long ChatId { get; init; }

    public required string Text { get; init; }

    public List<MessageEntity> Entities { get; init; } = [];
}
