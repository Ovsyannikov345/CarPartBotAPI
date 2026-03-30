using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Contexts;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Domain.Constants.Enums;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Utilities;

namespace CarPartBotApi.Application.Handlers;

internal class StartCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    IOptionsSnapshot<TelegramSettings> _options)
    : CommandHandlerBase(_dbContext, _telegramClient), ICommandHandler
{
    public bool CanHandle(Command command)
    {
        return command.CommandName is CommandNames.Start;
    }

    // TODO improve.
    public async Task<Result> Handle(Command command, UserContext userContext, ChatContext chatContext, CancellationToken ct)
    {
        var userLoadResult = await LoadUser(userContext, ct);

        if (userLoadResult.IsSuccess)
        {
            return await Respond(
                $"Happy to see you again, {userContext.FirstName}. You're already registered so feel free to write commands " +
                $"or type /help to see the list of supported commands.",
                userContext,
                chatContext,
                ct);
        }

        // TODO add datetime and soft delete interceptors.
        var user = new User
        {
            TelegramId = userContext.TelegramId,
            IsAdmin = userContext.TelegramId == _options.Value.AdminExternalId,
            CreatedAt = DateTime.UtcNow,
            UserInteractionState = new UserInteractionState
            {
                ActionType = ActionType.None,
                ActionStep = ActionStep.None,
                ActionState = JsonDocument.Parse("{}"),
                CreatedAt = DateTime.UtcNow
            }
        };

        DbContext.Add(user);
        await DbContext.SaveChangesAsync(ct);

        // TODO app capabilities.
        return await Respond(
                $"Nice to meet you, {userContext.FirstName}. You've been registered so feel free to write commands " +
                $"or type /help to see the list of supported commands.",
                userContext,
                chatContext,
                ct);
    }
}
