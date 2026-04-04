using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Contexts;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace CarPartBotApi.Application.Handlers;

// TODO add logging.
public abstract class CommandHandlerBase(IApplicationDbContext _dbContext, ITelegramClient _telegramClient)
{
    // TODO introduce TelegramContextAccessor
    public abstract string CommandName { get; }

    public abstract string CommandDescription { get; }

    public abstract bool AdminOnly { get; }

    protected readonly IApplicationDbContext DbContext = _dbContext;

    protected async Task<Result<User>> LoadUser(UserContext userContext, CancellationToken ct)
    {
        var user = await DbContext
            .Query<User>()
            .FirstOrDefaultAsync(u => u.TelegramId == userContext.TelegramId, ct);

        if (user is not null)
        {
            return user;
        }

        return Result<User>.Fail("User is not found.");
    }

    protected async Task<Result> Respond(string message, UserContext userContext, ChatContext chatContext, CancellationToken ct)
    {
        var responseResult = await _telegramClient.SendMessage(chatContext.Id, message, ct);

        if (responseResult.IsFailure)
        {
            return responseResult;
        }

        return Result.Succeed();
    }

    protected async Task<Result> HandleFailure(CommonHandlingFailureType failureType, UserContext userContext, ChatContext chatContext, CancellationToken ct)
    {
        var message = failureType switch
        {
            CommonHandlingFailureType.Unauthorized =>
                $"Looks like you're not registered. If so, please be polite and present yourself using /{CommandNames.Start} command.",

            CommonHandlingFailureType.ActionNotAllowed =>
                $"You're not allowed to execute this action. Use /{CommandNames.Help} to see the list of allowed commands.",

            _ => throw new ArgumentOutOfRangeException(nameof(failureType), failureType, "Unsupported failure type")
        };

        var responseResult = await _telegramClient.SendMessage(chatContext.Id, message, ct);

        if (responseResult.IsFailure)
        {
            return responseResult;
        }

        return Result.Succeed();
    }
}
