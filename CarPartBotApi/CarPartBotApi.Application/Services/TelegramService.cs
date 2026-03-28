using CarPartBotApi.Domain.Interfaces.Services;

namespace CarPartBotApi.Application.Services;

internal class TelegramService : ITelegramService
{
    public async Task ProcessTelegramEvent(string rawNotification, CancellationToken ct)
    {
        Console.WriteLine(rawNotification);

        return;
    }
}
