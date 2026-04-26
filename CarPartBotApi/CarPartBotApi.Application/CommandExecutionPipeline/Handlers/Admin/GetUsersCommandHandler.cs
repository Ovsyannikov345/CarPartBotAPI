using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Utilities;

namespace CarPartBotApi.Application.CommandExecutionPipeline.Handlers.Admin;

public class GetUsersCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    ITelegramContextAccessor _telegramContextAccessor,
    IFailureHandler _failureHandler)
    : CommandHandler<EmptyCommandState>(_dbContext, _telegramClient, _telegramContextAccessor, _failureHandler)
{
    public override string CommandName => CommandNames.Users;

    public override string CommandDescription => CommandDescriptions.Users;

    public override CommandAccessLevel CommandAccessLevel => CommandAccessLevel.AdminUserOnly;

    public override CommandType CommandType => CommandType.GetUsers;

    public override bool CanHandle(Command command)
    {
        return command.CommandName == CommandName;
    }

    protected override async Task<Result> Execute(Command command, CancellationToken ct)
    {
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
