using CarPartBotApi.Application.Contexts;
using CarPartBotApi.Application.Dto;
using Utilities;

namespace CarPartBotApi.Application.Handlers;

public interface ICommandHandler
{
    public bool CanHandle(Command command);

    public Task<Result> Handle(Command command, UserContext userContext, ChatContext chatContext, CancellationToken ct);
}
