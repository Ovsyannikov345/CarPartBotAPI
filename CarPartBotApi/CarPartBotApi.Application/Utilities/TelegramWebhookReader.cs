using CarPartBotApi.Application.Constants;
using CarPartBotApi.Application.Contexts;
using CarPartBotApi.Application.Dto;
using CarPartBotApi.Application.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utilities;

namespace CarPartBotApi.Application.Utilities;

// TODO proper null handling.
public sealed class TelegramWebhookReader : ICommandDataReader
{
    private static readonly JsonSerializerOptions DeserializationOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly TelegramWebhookNotificationDto _notification;

    private TelegramWebhookReader(TelegramWebhookNotificationDto notification)
    {
        _notification = notification;
    }

    public static Result<TelegramWebhookReader> Create(string rawWebhookPayload)
    {
        try
        {
            var deserializedNotification = JsonSerializer.Deserialize<TelegramWebhookNotificationDto>(rawWebhookPayload, DeserializationOptions);

            if (deserializedNotification is null)
            {
                // TODO log
                return Result<TelegramWebhookReader>.Fail("Failed to parse telegram webhook notification.");
            }

            return new TelegramWebhookReader(deserializedNotification);
        }
        catch
        {
            // TODO log
            return Result<TelegramWebhookReader>.Fail("Failed to parse telegram webhook notification.");
        }
    }

    public string? GetMessageText()
    {

        return _notification?.Message?.Text;
    }

    public bool HasCommands()
    {
        return _notification.Message?.Entities?.Any(e => e.Type == MessageEntityTypes.BotCommand) ?? false;
    }

    public List<Command> GetCommands()
    {
        if (!HasCommands())
        {
            return [];
        }

        var text = GetMessageText();

        if (string.IsNullOrEmpty(text))
        {
            return [];
        }

        var commandsCount = _notification.Message!.Entities!.Count(e => e.Type == MessageEntityTypes.BotCommand);

        List<Command> commands = new List<Command>(commandsCount);

        var messageEntities = _notification.Message.Entities!.OrderBy(e => e.Offset).ToArray();

        for (var i = 0; i < messageEntities.Length - 1; i++)
        {
            if (messageEntities[i].Type == MessageEntityTypes.BotCommand)
            {
                var argumentStart = messageEntities[i].Offset + messageEntities[i].Length + 1;

                var argumentEnd = messageEntities[i + 1].Offset - (messageEntities[i].Offset + messageEntities[i].Length + 1);

                commands.Add(new Command
                {
                    CommandName = text.Substring(messageEntities[i].Offset, messageEntities[i].Length).TrimStart('/'),
                    Argument = text.Substring(argumentStart, argumentEnd)
                });
            }
        }

        if (messageEntities[^1].Type == MessageEntityTypes.BotCommand)
        {
            if (messageEntities[^1].Offset + messageEntities[^1].Length == text.Length)
            {
                commands.Add(new Command
                {
                    CommandName = text.Substring(messageEntities[^1].Offset, messageEntities[^1].Length).TrimStart('/'),
                });
            }
            else
            {
                commands.Add(new Command
                {
                    CommandName = text.Substring(messageEntities[^1].Offset, messageEntities[^1].Length).TrimStart('/'),
                    Argument = text[(messageEntities[^1].Offset + messageEntities[^1].Length + 1)..]
                });
            }
        }

        return commands;
    }

    public UserContext ExtractUserContext()
    {
        var user = _notification?.Message?.From ?? _notification?.CallbackQuery?.From;

        ArgumentNullException.ThrowIfNull(user);

        return new UserContext
        {
            TelegramId = user.Id,
            FirstName = user.FirstName,
            LanguageCode = user.LanguageCode ?? "en"
        };
    }

    public ChatContext ExtractChatContext()
    {
        var chat = _notification?.Message?.Chat ?? _notification?.CallbackQuery?.Message?.Chat;

        ArgumentNullException.ThrowIfNull(chat);

        return new ChatContext
        {
            Id = chat.Id,
            Type = chat.Type
        };
    }

    public TelegramCallbackQuery? GetCallbackQuery()
    {
        var callbackQuery = _notification?.CallbackQuery;

        if (callbackQuery is null || callbackQuery.Data is null)
        {
            return null;
        }

        var parts = callbackQuery.Data.Split(':');

        return new TelegramCallbackQuery 
        { 
            Id = callbackQuery.Id, 
            Payload = new TelegramCallbackPayload 
            { 
                CommandName = parts[0], 
                Payload = parts[1] }
        };
    }
}
