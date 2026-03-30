using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace CarPartBotApi.Application.Configuration;

public sealed record TelegramSettings
{
    public const string SectionName = "Telegram";

    [Required]
    public long AdminExternalId { get; init; }

    [Required]
    [Url]
    public required string ApiBaseUrl { get; init; }

    [Required]
    public required string BotToken { get; init; }

    [Required]
    [ValidateObjectMembers]
    public required TelegramWebhookSettings Webhook { get; init; }
}

public sealed record TelegramWebhookSettings
{
    [Required]
    public required string SecretToken { get; init; }

    [Required]
    public int MaxConnections { get; init; }

    [Required]
    [Range(0, int.MaxValue, MinimumIsExclusive = true)]
    public int RegistrationPeriodInMinutes { get; init; }
}
