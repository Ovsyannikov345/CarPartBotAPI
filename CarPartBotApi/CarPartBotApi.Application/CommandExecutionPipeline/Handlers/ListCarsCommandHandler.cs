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

namespace CarPartBotApi.Application.CommandExecutionPipeline.Handlers;

public sealed class ListCarsCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    ITelegramContextAccessor _telegramContextAccessor,
    IFailureHandler _failureHandler)
    : CommandHandler<EmptyCommandState>(_dbContext, _telegramClient, _telegramContextAccessor, _failureHandler)
{
    public override string CommandName => CommandNames.ListCars;

    public override string CommandDescription => CommandDescriptions.ListCars;

    public override CommandAccessLevel CommandAccessLevel => CommandAccessLevel.AuthorizedUsersOnly;

    public override CommandType CommandType => CommandType.ListCars;

    public override bool CanHandle(Command command)
    {
        return command.CommandName == CommandName;
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
                $"You haven't added any cars yet. Type /{CommandNames.AddCar} command to add a new car.",
                ct);
        }

        var sb = new StringBuilder("Here's the list of your cars:\n");

        for (var i = 0; i < cars.Count; i++)
        {
            sb.AppendLine($"{i+1}. {cars[i].Brand} {cars[i].Model} (created {DateOnly.FromDateTime(cars[i].CreatedAt)})");
        }

        return await Respond(sb.ToString(), ct);
    }
}
