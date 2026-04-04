using CarPartBotApi.Application.Logging;
using Microsoft.Extensions.Logging;

namespace CarPartBotApi.Application.Background.TelegramWebhookProcessing;

public interface ITelegramWebhookEventDispatcher
{
    ValueTask EnqueueForProcessing(string rawPaylaod, CancellationToken ct);
}

public class TelegramWebhookEventDispatcher(
    ITelegramWebhookEventWriter _queueWriter,
    ILogger<TelegramWebhookEventDispatcher> _logger)
    : ITelegramWebhookEventDispatcher
{
    public async ValueTask EnqueueForProcessing(string rawPaylaod, CancellationToken ct)
    {
        await _queueWriter.Write(new TelegramWebhookMessage(rawPaylaod), ct);

        _logger.EnqueuedTelegramWebhookEvent(rawPaylaod);
    }
}
