namespace CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.SendMessage;

internal sealed record ReplyMarkup
{
    public List<List<InlineKeyboardButton>>? InlineKeyboard { get; init; }
}

internal sealed record InlineKeyboardButton
{
    public required string Text { get; init; }

    public required string CallbackData { get; init; }
}
