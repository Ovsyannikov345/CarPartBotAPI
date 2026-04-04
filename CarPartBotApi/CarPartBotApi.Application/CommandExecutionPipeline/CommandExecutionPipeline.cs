using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Application.Logging;
using Microsoft.Extensions.Logging;
using Utilities;

namespace CarPartBotApi.Application.CommandExecutionPipeline;

public interface ICommandExecutionPipeline
{
    public Task<Result> Execute(CancellationToken ct);
}

// TODO rework logging completely from the bottom.
public sealed class CommandExecutionPipeline(
    ITelegramContextAccessor _telegramContextAccessor,
    IEnumerable<Func<ITelegramContextAccessor, Result>> _validators,
    IEnumerable<ICommandHandler> _commandHandlers,
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
        else
        {
            // TODO use message handler(s)
            return Result.Succeed();
        }
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
