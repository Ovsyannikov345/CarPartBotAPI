using CarPartBotApi.Application.Clients.Telegram;
using CarPartBotApi.Application.Clients.Telegram.Dto;
using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Infrastructure.Clients.Abstractions;
using CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.AnswerCallbackQuery;
using CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.Common;
using CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.RegisterWebhook;
using CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.SendMessage;
using CarPartBotApi.Infrastructure.Clients.Telegram.Contracts.SetMyCommands;
using CarPartBotApi.Infrastructure.Constants.Telegram;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utilities;

namespace CarPartBotApi.Infrastructure.Clients.Telegram;

public sealed class TelegramClient(
    HttpClient _httpClient,
    IOptionsSnapshot<InfrastructureSettings> _infrastructureOptions,
    IOptionsSnapshot<TelegramSettings> _telegramOptions,
    ILogger<TelegramClient> _logger)
    : BaseClient(_httpClient, _logger), ITelegramClient
{
    private const string WebhookRegistrationEndpointUrl = "setWebhook";

    private const string CommandListUpdateEndpointUrl = "setMyCommands";

    private const string SendMessageEndpointUrl = "sendMessage";

    private const string AnswerCallbackQueryEndpointUrl = "answerCallbackQuery";

    protected override JsonSerializerOptions SerializerOptions => new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<Result> RegisterWebhook(CancellationToken ct)
    {
        if (_infrastructureOptions.Value.WebhookEndpointUrl is null)
        {
            return Result.Fail("Webhook URL is not configured.");
        }

        var webhookUrl = $"{_infrastructureOptions.Value.ApiBaseUrl}/{_infrastructureOptions.Value.WebhookEndpointUrl}";

        var request = new RegisterWebhookRequest
        {
            Url = webhookUrl,
            AllowedUpdates = [AllowedUpdateTypes.Message, AllowedUpdateTypes.CallbackQuery],
            SecretToken = _telegramOptions.Value.Webhook.SecretToken,
            MaxConnections = _telegramOptions.Value.Webhook.MaxConnections
        };

        using var response = await HttpClient.PostAsJsonAsync(WebhookRegistrationEndpointUrl, request, SerializerOptions, ct);

        if (response.IsSuccessStatusCode)
        {
            return Result.Succeed();
        }

        return await HandleError<TelegramApiErrorResponse>(response, ct);
    }

    public async Task<Result> UpdateBotCommandsList(List<BotCommand> botCommands, CancellationToken ct)
    {
        var request = new SetMyCommandsRequest
        {
            Commands = [.. botCommands.Select(c => new TelegramBotCommand
            {
                Command = c.Command,
                Description = c.Description
            })]
        };

        using var response = await HttpClient.PostAsJsonAsync(CommandListUpdateEndpointUrl, request, SerializerOptions, ct);

        if (response.IsSuccessStatusCode)
        {
            return Result.Succeed();
        }

        return await HandleError<TelegramApiErrorResponse>(response, ct);
    }

    public async Task<Result> AnswerCallbackQuery(string callbackQueryId, string text, CancellationToken ct)
    {
        var request = new AnswerCallbackQueryRequest
        {
            CallbackQueryId = callbackQueryId,
            Text = text,
        };

        using var response = await HttpClient.PostAsJsonAsync(AnswerCallbackQueryEndpointUrl, request, SerializerOptions, ct);

        if (response.IsSuccessStatusCode)
        {
            return Result.Succeed();
        }

        return await HandleError<TelegramApiErrorResponse>(response, ct);
    }

    public async Task<Result> SendMessage(long chatId, string message, CancellationToken ct)
    {
        var request = new SendMessageRequest
        {
            ChatId = chatId,
            Text = message,
            Entities = []
        };

        return await SendMessage(request, ct);
    }

    public async Task<Result> SendMessageWithButtons(long chatId, string message, List<List<InlineButton>> inlineButtons, CancellationToken ct)
    {
        var request = new SendMessageRequest
        {
            ChatId = chatId,
            Text = message,
            Entities = [],
            ReplyMarkup = new ReplyMarkup
            {
                InlineKeyboard =
                [..
                    inlineButtons.Select(row =>
                        row.Select(button =>
                            new InlineKeyboardButton
                            {
                                Text = button.Text,
                                CallbackData = $"{button.CallbackData.CommandName}:{button.CallbackData.Payload}"
                            })
                        .ToList()
                    )
                ]
            }
        };

        return await SendMessage(request, ct);
    }

    private async Task<Result> SendMessage(SendMessageRequest request, CancellationToken ct)
    {
        using var response = await HttpClient.PostAsJsonAsync(SendMessageEndpointUrl, request, SerializerOptions, ct);

        if (response.IsSuccessStatusCode)
        {
            return Result.Succeed();
        }

        return await HandleError<TelegramApiErrorResponse>(response, ct);
    }
}
