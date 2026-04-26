using CarPartBotApi.Infrastructure.Clients.Abstractions;

namespace CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.Common;

internal sealed record TelegramApiErrorResponse : ErrorResponse
{
    public required bool Ok { get; init; }

    public required int ErrorCode { get; init; }

    public required string Description { get; init; }

    public override string GetErrorMessage() => Description;
}
