using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Constants.Enums;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Application.Logging;
using Microsoft.Extensions.Logging;
using Utilities;

namespace CarPartBotApi.Application.Handlers;

public sealed class FailureHandler(
    ITelegramClient _telegramClient,
    ITelegramContextAccessor _telegramContextAccessor,
    ILogger<FailureHandler> _logger)
    : IFailureHandler
{
    public async Task<Result> Handle(HandlingFailureType failureType, CancellationToken ct)
    {
        try
        {
            var chatContext = _telegramContextAccessor.ChatContext;

            var message = failureType switch
            {
                HandlingFailureType.Unknown =>
                    $"Unknown error occurred while processing your request. Please, try again later.",

                HandlingFailureType.UnknownCommand =>
                    $"The command is not recognized. Use /{CommandNames.Help} to see the list of allowed commands.",

                HandlingFailureType.Unauthorized =>
                    $"Looks like you're not registered. If so, please be polite and present yourself using /{CommandNames.Start} command.",

                HandlingFailureType.ActionNotAllowed =>
                    $"You're not allowed to execute this action. Use /{CommandNames.Help} to see the list of allowed commands.",

                _ => throw new ArgumentOutOfRangeException(nameof(failureType), failureType, "Unsupported failure type")
            };

            var result = await _telegramClient.SendMessage(chatContext.Id, message, ct);

            if (result.IsFailure)
            {
                _logger.FailedToGracefullyHandleFailure(result.ErrorMessage);
            }
            else
            {
                _logger.FailureHandledGracefully(failureType);
            }

            return result;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.FailedToGracefullyHandleFailure(ex);

            return Result.Fail("Unexpected error");
        }
    }
}
