namespace CarPartBotApi.Application.Background.Abstractions;

public record MessageBase
{
    public required string CorrelationId { get; init; }
}
