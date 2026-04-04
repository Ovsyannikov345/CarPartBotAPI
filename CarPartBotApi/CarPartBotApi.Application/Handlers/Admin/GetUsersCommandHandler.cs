using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Utilities;

namespace CarPartBotApi.Application.Handlers.Admin;

internal class GetUsersCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    ITelegramContextAccessor _telegramContextAccessor,
    IFailureHandler _failureHandler)
    : CommandHandlerBase(_dbContext, _telegramClient, _telegramContextAccessor), ICommandHandler
{
    public override string CommandName => CommandNames.Users;

    public override string CommandDescription => CommandDescriptions.Users;

    public override bool AdminOnly => true;

    public bool CanHandle(Command command)
    {
        return command.CommandName is CommandNames.Users;
    }

    public async Task<Result> Handle(Command command, CancellationToken ct)
    {
        var userLoadResult = await LoadUser(ct);

        if (userLoadResult.IsFailure)
        {
            return await _failureHandler.Handle(HandlingFailureType.Unauthorized, ct);
        }

        var user = userLoadResult.Value;

        if (!user.IsAdmin)
        {
            return await _failureHandler.Handle(HandlingFailureType.ActionNotAllowed, ct);
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

        return await Respond(sb.ToString(), ct);
    }
}
