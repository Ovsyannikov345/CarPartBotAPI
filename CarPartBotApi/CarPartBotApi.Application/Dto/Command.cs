namespace CarPartBotApi.Application.Dto;

public sealed record Command
{
    public required string CommandName { get; init; }

    public string? Argument { get; init; }
}
