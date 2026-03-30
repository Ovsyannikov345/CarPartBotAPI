namespace CarPartBotApi.Application.Contexts;

public sealed record ChatContext
{
    public long Id { get; init; }

    public required string Type { get; init; }
}
