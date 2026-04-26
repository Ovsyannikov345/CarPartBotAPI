namespace CarPartBotApi.Application.Dto;

public sealed record TelegramCallbackQuery
{
    public required string Id { get; init; }

    public required TelegramCallbackPayload Payload { get; init; }
}

public sealed record TelegramCallbackPayload
{
    public required string CommandName { get; init; }

    public required string Payload { get; init; }
}
