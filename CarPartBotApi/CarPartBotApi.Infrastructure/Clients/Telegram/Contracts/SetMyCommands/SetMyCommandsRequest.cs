namespace CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.SetMyCommands;

internal sealed record SetMyCommandsRequest
{
    public required List<TelegramBotCommand> Commands { get; init; }
}

internal sealed record TelegramBotCommand
{
    public required string Command { get; init; }

    public required string Description { get; init; }
}
