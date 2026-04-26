using CarPartBotApi.Application.Dto;

namespace CarPartBotApi.Application.Clients.Telegram.Dto;

public sealed record InlineButton
{
    public required string Text { get; init; }

    public required TelegramCallbackPayload CallbackData { get; init; }
}
