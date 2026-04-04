using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Contexts;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Utilities;

namespace CarPartBotApi.Application.Handlers.Admin;

internal class GetUsersCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient)
    : CommandHandlerBase(_dbContext, _telegramClient), ICommandHandler
{
    public override string CommandName => CommandNames.Users;

    public override string CommandDescription => CommandDescriptions.Users;

    public override bool AdminOnly => true;

    public bool CanHandle(Command command)
    {
        return command.CommandName is CommandNames.Users;
    }

    public async Task<Result> Handle(Command command, UserContext userContext, ChatContext chatContext, CancellationToken ct)
    {
        var userLoadResult = await LoadUser(userContext, ct);

        if (userLoadResult.IsFailure)
        {
            return await HandleFailure(CommonHandlingFailureType.Unauthorized, userContext, chatContext, ct);
        }

        var user = userLoadResult.Value;

        if (!user.IsAdmin)
        {
            return await HandleFailure(CommonHandlingFailureType.ActionNotAllowed, userContext, chatContext, ct);
        }

        var users = await DbContext
            .Query<User>()
            .AsNoTracking()
            .ToListAsync(ct);

        var sb = new StringBuilder("Registered users:\n");

        foreach (var registeredUser in users)
        {
            sb.AppendLine();
            sb.AppendLine(
                $"ID: {registeredUser.Id}\n" +
                $"Telegram ID: {registeredUser.TelegramId}\n" +
                $"Admin: {registeredUser.IsAdmin}\n" +
                $"Registration date: {registeredUser.CreatedAt}");
        }

        return await Respond(sb.ToString(), userContext, chatContext, ct);
    }
}
