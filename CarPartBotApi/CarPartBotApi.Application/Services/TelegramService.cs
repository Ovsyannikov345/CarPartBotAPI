using CarPartBotApi.Application.CommandExecutionPipeline;
using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Logging;
using CarPartBotApi.Application.Utilities;
using Microsoft.Extensions.Logging;
using Utilities;

namespace CarPartBotApi.Application.Services;

public interface ITelegramService
{
    public Task<Result> ProcessTelegramEvent(string rawNotification, CancellationToken ct);
}

internal class TelegramService(
    ICommandExecutionPipelineBuilder _commandExecutionPipelineBuilder,
    ILogger<TelegramService> _logger)
    : ITelegramService
{
    public async Task<Result> ProcessTelegramEvent(string rawNotification, CancellationToken ct)
    {
        // TODO allow only one request per user (synchronize) to maintain proper interaction state.

        var readerCreationResult = TelegramWebhookReader.Create(rawNotification);

        if (readerCreationResult.IsFailure)
        {
            _logger.FailedToCreateWebhookReader(rawNotification);

            return readerCreationResult;
        }

        var reader = readerCreationResult.Value;

        var commandExecutionPipeline = _commandExecutionPipelineBuilder
            .WithDataReader(reader)
            .WithValidator((telegramContextAccessor) =>
            {
                if (telegramContextAccessor.ChatContext.Type is not ChatTypes.Private)
                {
                    return Result.Fail("Only private chats are supported");
                }

                return Result.Succeed();
            })
            .Build();

        var executionResult = await commandExecutionPipeline.Execute(ct);

        if (executionResult.IsFailure)
        {
            _logger.FailedToHandleTelegramWebhookEvent("Command execution pipeline has failed");

            return Result.Fail("Failed to process received event");
        }

        _logger.HandledTelegramWebhookEvent();

        return executionResult;
    }
}
