using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Utilities;

namespace CarPartBotApi.Application.CommandExecutionPipeline.Handlers;

public sealed class HelpCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    IServiceProvider _serviceProvider,
    ITelegramContextAccessor _telegramContextAccessor,
    IFailureHandler _failureHandler)
    : CommandHandler<EmptyCommandState>(_dbContext, _telegramClient, _telegramContextAccessor, _failureHandler)
{
    public override string CommandName => CommandNames.Help;

    public override string CommandDescription => CommandDescriptions.Help;

    public override CommandAccessLevel CommandAccessLevel => CommandAccessLevel.AuthorizedUsersOnly;

    public override CommandType CommandType => CommandType.Help;

    public override bool CanHandle(Command command)
    {
        return command.CommandName == CommandName;
    }

    protected override async Task<Result> Execute(Command command, CancellationToken ct)
    {
        var user = GetUser();

        var commandHandlers = _serviceProvider
            .GetServices<ICommandHandler>()
            .Where(handler => handler is not StartCommandHandler);

        if (!user.IsAdmin)
        {
            commandHandlers = commandHandlers.Where(handler => handler.CommandAccessLevel is not CommandAccessLevel.AdminUserOnly);
        }

        var sb = new StringBuilder("Here's the list of available commands:\n");

        foreach (var handler in commandHandlers)
        {
            sb.AppendLine($"/{handler.CommandName} - {handler.CommandDescription}.");
        }

        return await Respond(sb.ToString(), ct);
    }
}
