using Utilities;

namespace CarPartBotApi.Application.Clients.Telegram;

public interface ITelegramClient
{
    public Task<Result> RegisterWebhook(CancellationToken ct);

    public Task<Result> SendMessage(long chatId, string message, CancellationToken ct);
}
