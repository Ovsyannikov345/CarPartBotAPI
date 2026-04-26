using CarPartBotApi.Application.Constants.Enums;

namespace CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;

public record CommandState(CommandType CommandType);
