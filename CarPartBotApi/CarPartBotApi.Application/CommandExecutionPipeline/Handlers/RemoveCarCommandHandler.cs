using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Clients.Telegram.Dto;
using CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace CarPartBotApi.Application.CommandExecutionPipeline.Handlers;

public sealed class RemoveCarCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    ITelegramContextAccessor _telegramContextAccessor,
    IFailureHandler _failureHandler)
    : CommandHandler<EmptyCommandState>(_dbContext, _telegramClient, _telegramContextAccessor, _failureHandler), ICallbackableHandler
{
    public override string CommandName => CommandNames.RemoveCar;

    public override string CommandDescription => CommandDescriptions.RemoveCar;

    public override CommandAccessLevel CommandAccessLevel => CommandAccessLevel.AuthorizedUsersOnly;

    public override CommandType CommandType => CommandType.RemoveCar;

    public override bool CanHandle(Command command)
    {
        return command.CommandName == CommandName;
    }

    public bool CanHandleCallback(TelegramCallbackPayload callbackPayload)
    {
        return callbackPayload.CommandName == CommandName;
    }

    public async Task<Result> HandleCallback(TelegramCallbackQuery callbackQuery, CancellationToken ct)
    {
        if (!Guid.TryParse(callbackQuery.Payload.Payload, out var carIdToRemove))
        {
            return Result.Fail("Failed to parse callback payload.");
        }

        var car = await DbContext
            .Query<Car>()
            .FirstOrDefaultAsync(c => c.Id == carIdToRemove, ct);

        if (car is null)
        {
            return await TelegramClient.AnswerCallbackQuery(callbackQuery.Id, "Selected car is not found.", ct);
        }

        car.DeletedAt = DateTime.UtcNow;
        await DbContext.SaveChangesAsync(ct);

        return await TelegramClient.AnswerCallbackQuery(callbackQuery.Id, $"Car '{car.Brand} {car.Model}' has been removed.", ct);
    }

    protected override async Task<Result> Execute(Command command, CancellationToken ct)
    {
        var user = GetUser();

        var cars = await DbContext
            .Query<Car>()
            .AsNoTracking()
            .Where(c => c.UserId == user.Id)
            .ToListAsync(ct);

        if (cars.Count == 0)
        {
            return await TelegramClient.SendMessage(
                TelegramContextAccessor.ChatContext.Id, 
                $"You have no saved cars. Add a new car using /{CommandNames.AddCar} command.",
                ct);
        }

        var buttons = cars.Select(c => new List<InlineButton> 
        { 
            new() {
                Text = $"{c.Brand} {c.Model}",
                CallbackData = new TelegramCallbackPayload
                {
                    CommandName = CommandName,
                    Payload = c.Id.ToString()
                } 
            }
        }).ToList();

        return await TelegramClient.SendMessageWithButtons(
            TelegramContextAccessor.ChatContext.Id,
            "Which of the cars do you want to remove?",
            buttons,
            ct);
    }
}
