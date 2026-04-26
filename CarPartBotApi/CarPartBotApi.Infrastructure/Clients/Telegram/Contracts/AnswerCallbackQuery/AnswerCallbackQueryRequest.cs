namespace CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.AnswerCallbackQuery;

internal sealed record AnswerCallbackQueryRequest
{
    public required string CallbackQueryId { get; init; }

    public required string Text { get; init; }
}
