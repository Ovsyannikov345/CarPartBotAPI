using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Contexts;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace CarPartBotApi.Application.Handlers;

// TODO add logging.
public abstract class CommandHandlerBase(IApplicationDbContext _dbContext, ITelegramClient _telegramClient)
{
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
}
