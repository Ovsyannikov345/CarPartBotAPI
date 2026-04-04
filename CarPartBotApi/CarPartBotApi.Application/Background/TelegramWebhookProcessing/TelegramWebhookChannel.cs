using System.Threading.Channels;

namespace CarPartBotApi.Application.Background.TelegramWebhookProcessing;

public interface ITelegramWebhookEventWriter
{
    public ValueTask Write(TelegramWebhookMessage message, CancellationToken ct);
}

public interface ITelegramWebhookEventReader
{
    public IAsyncEnumerable<TelegramWebhookMessage> ReadAll(CancellationToken ct);
}

public sealed class TelegramWebhookChannel
    : ITelegramWebhookEventWriter, ITelegramWebhookEventReader
{
    private readonly Channel<TelegramWebhookMessage> _channel;

    public TelegramWebhookChannel()
    {
        _channel = Channel.CreateUnbounded<TelegramWebhookMessage>();
    }

    public ValueTask Write(TelegramWebhookMessage message, CancellationToken ct)
    {
        return _channel.Writer.WriteAsync(message, ct);
    }

    public IAsyncEnumerable<TelegramWebhookMessage> ReadAll(CancellationToken ct)
    {
        return _channel.Reader.ReadAllAsync(ct);
    }
}

public sealed record TelegramWebhookMessage(string RawPayload);
