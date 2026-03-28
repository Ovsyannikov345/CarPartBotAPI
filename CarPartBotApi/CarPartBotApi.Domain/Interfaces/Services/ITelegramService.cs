namespace CarPartBotApi.Domain.Interfaces.Services;

public interface ITelegramService
{
    public Task ProcessTelegramEvent(string rawNotification, CancellationToken ct);
}
