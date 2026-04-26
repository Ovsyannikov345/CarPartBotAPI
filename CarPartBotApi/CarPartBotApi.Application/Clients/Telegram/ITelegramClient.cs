using CarPartBotApi.Application.Clients.Telegram.Dto;
using Utilities;

namespace CarPartBotApi.Application.Clients.Telegram;

public interface ITelegramClient
{
    public Task<Result> RegisterWebhook(CancellationToken ct);

    public Task<Result> UpdateBotCommandsList(List<BotCommand> botCommands, CancellationToken ct);

    public Task<Result> AnswerCallbackQuery(string callbackQueryId, string text, CancellationToken ct);

    public Task<Result> SendMessage(long chatId, string message, CancellationToken ct);

    public Task<Result> SendMessageWithButtons(long chatId, string message, List<List<InlineButton>> inlineButtons, CancellationToken ct);
}
