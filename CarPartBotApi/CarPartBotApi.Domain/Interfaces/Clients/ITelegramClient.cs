using Utilities;

namespace CarPartBotApi.Domain.Interfaces.Clients;

public interface ITelegramClient
{
    public Task<Result> RegisterWebhook(CancellationToken ct);
}
