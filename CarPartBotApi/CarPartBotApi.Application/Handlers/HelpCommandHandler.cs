using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Contexts;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Text;
using Utilities;

namespace CarPartBotApi.Application.Handlers;

public sealed class HelpCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    IServiceProvider _serviceProvider)
    : CommandHandlerBase(_dbContext, _telegramClient), ICommandHandler
{
    public override string CommandName => CommandNames.Help;

    public override string CommandDescription => CommandDescriptions.Help;

    public override bool AdminOnly => false;

    public bool CanHandle(Command command)
    {
        return command.CommandName is CommandNames.Help;
    }

    public async Task<Result> Handle(Command command, UserContext userContext, ChatContext chatContext, CancellationToken ct)
    {
        var userLoadResult = await LoadUser(userContext, ct);

        if (userLoadResult.IsFailure)
        {
            return await HandleFailure(CommonHandlingFailureType.Unauthorized, userContext, chatContext, ct);
        }

        var user = userLoadResult.Value;

        var commandHandlers = _serviceProvider
            .GetServices<ICommandHandler>()
            .Where(handler => handler is not StartCommandHandler);

        if (!user.IsAdmin)
        {
            commandHandlers = commandHandlers.Where(handler => handler.AdminOnly == false);
        }

        var sb = new StringBuilder("Here's the list of available commands:\n");

        foreach (var handler in commandHandlers)
        {
            sb.AppendLine($"/{handler.CommandName} - {handler.CommandDescription}.");
        }

        return await Respond(sb.ToString(), userContext, chatContext, ct);
    }
}
