using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Dto;
using Utilities;

namespace CarPartBotApi.Application.Interfaces;

public interface ICommandHandler
{
    public string CommandName { get; }

    public string CommandDescription { get; }

    public CommandAccessLevel CommandAccessLevel { get; }

    public bool CanHandle(Command command);

    public Task<Result> Handle(Command command, CancellationToken ct);
}
