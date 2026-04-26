using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utilities;

namespace CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;

// TODO add proper logging to commands.
public abstract class CommandHandler<TState>(
    IApplicationDbContext _dbContext,
    ITelegramClient _telegramClient,
    ITelegramContextAccessor _telegramContextAccessor,
    IFailureHandler _failureHandler)
    : ICommandHandler
    where TState : CommandState
{
    protected static readonly JsonSerializerOptions StateSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public abstract CommandType CommandType { get; }

    public abstract string CommandName { get; }

    public abstract string CommandDescription { get; }

    public abstract CommandAccessLevel CommandAccessLevel { get; }

    protected readonly IApplicationDbContext DbContext = _dbContext;

    protected readonly ITelegramContextAccessor TelegramContextAccessor = _telegramContextAccessor;

    protected readonly ITelegramClient TelegramClient = _telegramClient;

    protected bool IsAuthorized => _user is not null;

    protected TState? CommandState { get; set; } = null;

    private User? _user = null;

    public abstract bool CanHandle(Command command);

    public async Task<Result> Handle(Command command, CancellationToken ct)
    {
        _user = await LoadUser(ct);
        CommandState = LoadState();

        if (CommandAccessLevel is not CommandAccessLevel.Anonymous)
        {
            if (_user is null)
            {
                return await _failureHandler.Handle(HandlingFailureType.Unauthorized, ct);
            }

            if (CommandAccessLevel is CommandAccessLevel.AdminUserOnly && !_user.IsAdmin)
            {
                return await _failureHandler.Handle(HandlingFailureType.ActionNotAllowed, ct);
            }
        }

        var executionResult = await Execute(command, ct);

        await SaveState();

        return executionResult;
    }

    protected abstract Task<Result> Execute(Command command, CancellationToken ct);

    // TODO Remove this method. Use client directly
    protected async Task<Result> Respond(string message, CancellationToken ct)
    {
        var chatContext = TelegramContextAccessor.ChatContext;

        var responseResult = await TelegramClient.SendMessage(chatContext.Id, message, ct);

        if (responseResult.IsFailure)
        {
            return responseResult;
        }

        return Result.Succeed();
    }

    protected User GetUser()
    {
        ArgumentNullException.ThrowIfNull(_user);

        return _user;
    }

    protected async Task<User?> LoadUser(CancellationToken ct)
    {
        var userContext = TelegramContextAccessor.UserContext;

        return await DbContext
            .Query<User>()
            .Include(u => u.UserInteractionState)
            .FirstOrDefaultAsync(u => u.TelegramId == userContext.TelegramId, ct);
    }

    private TState? LoadState()
    {
        if (_user is null)
        {
            return null;
        }

        try
        {
            var state = JsonSerializer.Deserialize<TState>(_user.UserInteractionState.ActionState, StateSerializerOptions);

            var currentStateType = (state as CommandState)?.CommandType;

            if (currentStateType != CommandType)
            {
                return null;
            }

            return state;
        }
        catch
        {
            return null;
        }
    }

    private async Task SaveState()
    {
        if (_user is null)
        {
            return;
        }

        if (CommandState is null)
        {
            _user.UserInteractionState.ActionState = JsonSerializer.Serialize(new EmptyCommandState(), StateSerializerOptions);
        }
        else
        {
            _user.UserInteractionState.ActionState = JsonSerializer.Serialize(CommandState, StateSerializerOptions);
        }

        await DbContext.SaveChangesAsync();
    }
}
