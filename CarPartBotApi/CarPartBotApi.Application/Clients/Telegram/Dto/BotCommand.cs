namespace CarPartBotApi.Application.Clients.Telegram.Dto;

public sealed record BotCommand
{
    public required string Command { get; init; }

    public required string Description { get; init; }
}
