using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Handlers;
using CarPartBotApi.Application.Utilities;
using CarPartBotApi.Domain.Entities;
using Utilities;

namespace CarPartBotApi.Application.Services;

public interface ITelegramService
{
    public Task<Result> ProcessTelegramEvent(string rawNotification, CancellationToken ct);
}

// TODO add default handler
// TODO add error handler
internal class TelegramService(IEnumerable<ICommandHandler> commandHandlers) : ITelegramService
{
    public async Task<Result> ProcessTelegramEvent(string rawNotification, CancellationToken ct)
    {
        // TODO remove.
        Console.WriteLine(rawNotification);

        var readerCreationResult = TelegramWebhookReader.Create(rawNotification);

        if (readerCreationResult.IsFailure)
        {
            // TODO log.
            return readerCreationResult;
        }

        var reader = readerCreationResult.Value;

        var userContext = reader.ExtractUserContext();

        var chatContext = reader.ExtractChatContext();

        if (chatContext.Type is not ChatTypes.Private)
        {
            return Result.Fail("Only private chats are supported.");
        }

        var commands = reader.GetCommands();

        if (commands.Count > 0)
        {
            var command = commands.First();

            var commandHandler = commandHandlers.FirstOrDefault(h => h.CanHandle(command));

            if (commandHandler is null)
            {
                // TODO handle.
                Console.WriteLine("Handler is null");
                return Result.Succeed();
            }

            var commandResult = await commandHandler.Handle(command, userContext, chatContext, ct);

            if (commandResult.IsFailure)
            {
                // TODO handle
                Console.WriteLine($"Failed to handle: {commandResult.ErrorMessage}");
                return Result.Succeed();
            }
        }
        else
        {
            // Delegate to message handlers
        }

        return Result.Succeed();
    }
}
