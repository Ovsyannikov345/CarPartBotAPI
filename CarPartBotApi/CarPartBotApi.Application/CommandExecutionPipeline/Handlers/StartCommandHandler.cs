using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;
using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Utilities;

namespace CarPartBotApi.Application.CommandExecutionPipeline.Handlers;

internal class StartCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    ITelegramContextAccessor _telegramContextAccessor,
    IFailureHandler _failureHandler,
    IOptionsSnapshot<TelegramSettings> _options)
    : CommandHandler<EmptyCommandState>(_dbContext, _telegramClient, _telegramContextAccessor, _failureHandler)
{
    public override string CommandName => CommandNames.Start;

    public override string CommandDescription => CommandDescriptions.Start;

    public override CommandAccessLevel CommandAccessLevel => CommandAccessLevel.Anonymous;

    public override CommandType CommandType => CommandType.Start;

    public override bool CanHandle(Command command)
    {
        return command.CommandName == CommandName;
    }

    protected override async Task<Result> Execute(Command command, CancellationToken ct)
    {
        var userContext = TelegramContextAccessor.UserContext;

        if (IsAuthorized)
        {
            return await Respond(
                $"Happy to see you again, {userContext.FirstName}. You're already registered so feel free to write commands " +
                $"or type /{CommandNames.Help} to see the list of supported commands.",
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
                ActionState = JsonSerializer.Serialize(new EmptyCommandState(), StateSerializerOptions),
                CreatedAt = DateTime.UtcNow
            }
        };

        DbContext.Add(user);
        await DbContext.SaveChangesAsync(ct);

        // TODO app capabilities.
        return await Respond(
            $"Nice to meet you, {userContext.FirstName}. You've been registered so feel free to write commands " +
            $"or type /{CommandNames.Help} to see the list of supported commands.",
            ct);
    }
}
