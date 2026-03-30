using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Contexts;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Domain.Interfaces.Data;
using Utilities;

namespace CarPartBotApi.Application.Handlers;

public sealed class HelpCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient)
    : CommandHandlerBase(_dbContext, _telegramClient), ICommandHandler
{
    public bool CanHandle(Command command)
    {
        return command.CommandName is CommandNames.Help;
    }

    public async Task<Result> Handle(Command command, UserContext userContext, ChatContext chatContext, CancellationToken ct)
    {
        var userLoadResult = await LoadUser(userContext, ct);

        if (userLoadResult.IsFailure)
        {
            return userLoadResult;
        }

        return await Respond("Happy to help!", userContext, chatContext, ct);
    }
}
