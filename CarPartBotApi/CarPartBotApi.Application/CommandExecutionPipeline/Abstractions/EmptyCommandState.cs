using CarPartBotApi.Application.Constants.Enums;

namespace CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;

public sealed record EmptyCommandState() : CommandState(CommandType.None)
{
}
