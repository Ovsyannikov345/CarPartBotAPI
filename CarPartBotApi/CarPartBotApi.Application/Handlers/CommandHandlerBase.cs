using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace CarPartBotApi.Application.Handlers;

// TODO add proper logging to commands.
public abstract class CommandHandlerBase(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    ITelegramContextAccessor _telegramContextAccessor)
{
    public abstract string CommandName { get; }

    public abstract string CommandDescription { get; }

    public abstract bool AdminOnly { get; }

    protected readonly IApplicationDbContext DbContext = _dbContext;

    protected readonly ITelegramContextAccessor TelegramContextAccessor = _telegramContextAccessor;

    protected async Task<Result<User>> LoadUser(CancellationToken ct)
    {
        var userContext = TelegramContextAccessor.UserContext;

        var user = await DbContext
            .Query<User>()
            .FirstOrDefaultAsync(u => u.TelegramId == userContext.TelegramId, ct);

        if (user is not null)
        {
            return user;
        }

        return Result<User>.Fail("User is not found.");
    }

    protected async Task<Result> Respond(string message, CancellationToken ct)
    {
        var chatContext = TelegramContextAccessor.ChatContext;

        var responseResult = await _telegramClient.SendMessage(chatContext.Id, message, ct);

        if (responseResult.IsFailure)
        {
            return responseResult;
        }

        return Result.Succeed();
    }
}
