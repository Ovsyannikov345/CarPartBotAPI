namespace CarPartBotApi.Application.Contexts;

public sealed record UserContext
{
    public long TelegramId { get; init; }

    public required string FirstName { get; init; }

    public required string LanguageCode { get; init; }
}
