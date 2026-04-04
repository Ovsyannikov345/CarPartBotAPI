using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Logging;
using Microsoft.Extensions.Logging;

namespace CarPartBotApi.Application.Background.TelegramWebhookProcessing;

public interface ITelegramWebhookEventDispatcher
{
    ValueTask EnqueueForProcessing(string rawPaylaod, CancellationToken ct);
}

public class TelegramWebhookEventDispatcher(
    ITelegramWebhookEventWriter _queueWriter,
    ICorrelationIdAccessor _correlationIdAccessor,
    ILogger<TelegramWebhookEventDispatcher> _logger)
    : ITelegramWebhookEventDispatcher
{
    public async ValueTask EnqueueForProcessing(string rawPaylaod, CancellationToken ct)
    {
        await _queueWriter.Write(new TelegramWebhookMessage 
        { 
            RawPayload = rawPaylaod, 
            CorrelationId = _correlationIdAccessor.CorrelationId,
        }, ct);

        _logger.EnqueuedTelegramWebhookEvent(rawPaylaod);
    }
}
