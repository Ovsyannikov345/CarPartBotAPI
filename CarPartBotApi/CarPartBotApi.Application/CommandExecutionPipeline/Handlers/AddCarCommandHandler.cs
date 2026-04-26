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
using Utilities;

namespace CarPartBotApi.Application.CommandExecutionPipeline.Handlers;

internal sealed class AddCarCommandHandler(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    ITelegramContextAccessor _telegramContextAccessor,
    IFailureHandler _failureHandler)
    : CommandHandler<AddCarCommandState>(_dbContext, _telegramClient, _telegramContextAccessor, _failureHandler)
{
    public override CommandType CommandType => CommandType.AddCar;

    public override string CommandName => CommandNames.AddCar;

    public override string CommandDescription => CommandDescriptions.AddCar;

    public override CommandAccessLevel CommandAccessLevel => CommandAccessLevel.AuthorizedUsersOnly;

    public override bool CanHandle(Command command)
    {
        return command.CommandName == CommandName;
    }

    private const int MaxCarsCount = 5;

    protected override async Task<Result> Execute(Command command, CancellationToken ct)
    {
        var user = GetUser();

        // Step 1 - Initiate
        if (CommandState is null || CommandState.CommandType is not CommandType.AddCar)
        {
            CommandState = new AddCarCommandState();

            var carsCount = await DbContext
                .Query<Car>()
                .Where(c => c.UserId == user.Id)
                .CountAsync(ct);

            if (carsCount >= MaxCarsCount)
            {
                return await Respond($"You've reached a limit of {MaxCarsCount} cars. You can remove some of yourt cars by using /{CommandNames.RemoveCar} command.", ct);
            }

            return await Respond("Let's add a new car. What's the car brand?", ct);
        }

        // Step 2 - Specify brand
        if (CommandState.Brand is null)
        {
            var brand = command.Argument;

            if (string.IsNullOrEmpty(brand))
            {
                return await Respond("Invalid car brand. Valid examples: Nissan, Renault.", ct);
            }

            CommandState.Brand = brand;

            return await Respond("What's the car model?", ct);
        }

        // Step 3 - Specify model
        var model = command.Argument;

        if (string.IsNullOrEmpty(model) || model.Contains(CommandState.Brand, StringComparison.OrdinalIgnoreCase))
        {
            return await Respond("Invalid car model. Valid examples: Duster, 206 CC.", ct);
        }

        var car = new Car
        {
            Brand = CommandState.Brand,
            Model = model,
            User = GetUser(),
            CreatedAt = DateTime.UtcNow,
        };

        DbContext.Add(car);
        await DbContext.SaveChangesAsync(ct);

        CommandState = null;

        return await Respond($"Created new car. Use /{CommandNames.ListCars} command to see the list of your cars.", ct);
    }
}

internal sealed record AddCarCommandState() : CommandState(CommandType.AddCar)
{
    public string? Brand { get; set; }
}
