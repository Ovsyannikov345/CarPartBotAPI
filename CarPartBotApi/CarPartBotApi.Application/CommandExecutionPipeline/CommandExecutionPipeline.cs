using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Application.Logging;
using CarPartBotApi.Domain.Entities;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Utilities;

namespace CarPartBotApi.Application.CommandExecutionPipeline;

public interface ICommandExecutionPipeline
{
    public Task<Result> Execute(CancellationToken ct);
}

// TODO rework logging completely from the bottom.
public sealed class CommandExecutionPipeline(
    IApplicationDbContext _dbContext,
    ITelegramContextAccessor _telegramContextAccessor,
    IEnumerable<Func<ITelegramContextAccessor, Result>> _validators,
    IEnumerable<ICommandHandler> _commandHandlers,
    IEnumerable<ICallbackableHandler> _callbackHandlers,
    IFailureHandler _failureHandler,
    ILogger<CommandExecutionPipeline> _logger)
    : ICommandExecutionPipeline
{
    public async Task<Result> Execute(CancellationToken ct)
    {
        foreach (var validator in _validators)
        {
            var validationResult = validator(_telegramContextAccessor);

            if (validationResult.IsFailure)
            {
                return HandleFailure(validationResult.ErrorMessage);
            }
        }

        if (_telegramContextAccessor.Commands.Count > 0)
        {
            var command = _telegramContextAccessor.Commands[0];

            var commandHandler = _commandHandlers.FirstOrDefault(handler => handler.CanHandle(command));

            if (commandHandler is null)
            {
                _logger.CommandExecutionPipelineFailed($"Unknnown command '{command.CommandName}'");

                return await HandleFailure(HandlingFailureType.UnknownCommand, ct);
            }

            try
            {
                var handlingResult = await commandHandler.Handle(command, ct);

                if (handlingResult.IsFailure)
                {
                    return HandleFailure(handlingResult.ErrorMessage);
                }

                _logger.CommandExecutionPipelineSucceeded();

                return handlingResult;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.CommandExecutionPipelineFailed(ex);

                return await HandleFailure(HandlingFailureType.Unknown, ct);
            }
        }
        else if (_telegramContextAccessor.Message is not null)
        {
            // TODO it's just a temporary workaround.
            var userContext = _telegramContextAccessor.UserContext;

            var state = await _dbContext
                .Query<UserInteractionState>()
                .AsNoTracking()
                .Where(s => s.User.TelegramId == userContext.TelegramId)
                .FirstOrDefaultAsync(ct);

            if (state is null)
            {
                // TODO handle
                return Result.Succeed();
            }

            var commandState = JsonSerializer.Deserialize<CommandState>(state.ActionState);

            if (commandState is null)
            {
                // TODO handle
                return Result.Succeed();
            }

            var command = new Command 
            { 
                CommandName = CommandNames.AddCar, // TODO map
                Argument = _telegramContextAccessor.Message 
            };

            var handler = _commandHandlers.FirstOrDefault(h => h.CanHandle(command));

            if (handler is null)
            {
                _logger.CommandExecutionPipelineFailed($"Failed to determine handler for message");

                return await HandleFailure(HandlingFailureType.UnknownCommand, ct);
            }

            try
            {
                var handlingResult = await handler.Handle(command, ct);

                if (handlingResult.IsFailure)
                {
                    return HandleFailure(handlingResult.ErrorMessage);
                }

                _logger.CommandExecutionPipelineSucceeded();

                return handlingResult;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.CommandExecutionPipelineFailed(ex);

                return await HandleFailure(HandlingFailureType.Unknown, ct);
            }
        }
        else if (_telegramContextAccessor.CallbackQuery is not null)
        {
            var query = _telegramContextAccessor.CallbackQuery;

            var callbackHandler = _callbackHandlers.FirstOrDefault(handler => handler.CanHandleCallback(query.Payload));

            if (callbackHandler is null)
            {
                _logger.CommandExecutionPipelineFailed($"Unknnown payload '{query.Payload}'");

                return await HandleFailure(HandlingFailureType.Unknown, ct);
            }

            try
            {
                var handlingResult = await callbackHandler.HandleCallback(query, ct);

                if (handlingResult.IsFailure)
                {
                    return HandleFailure(handlingResult.ErrorMessage);
                }

                _logger.CommandExecutionPipelineSucceeded();

                return handlingResult;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.CommandExecutionPipelineFailed(ex);

                return await HandleFailure(HandlingFailureType.Unknown, ct);
            }
        }

        return Result.Fail("Telegram update is missing command, message and callback query");
    }

    private async Task<Result> HandleFailure(HandlingFailureType type, CancellationToken ct)
    {
        var failureHandlingResult = await _failureHandler.Handle(type, ct);

        if (failureHandlingResult.IsFailure)
        {
            return HandleFailure("Failed to handle command handler error gracefully");
        }

        return Result.Succeed();
    }

    private Result HandleFailure(string errorMessage)
    {
        _logger.CommandExecutionPipelineFailed(errorMessage);

        return Result.Fail(errorMessage);
    }
}
