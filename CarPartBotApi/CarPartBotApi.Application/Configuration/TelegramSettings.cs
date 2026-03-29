using System.ComponentModel.DataAnnotations;

namespace CarPartBotApi.Application.Configuration;

public sealed record TelegramSettings
{
    public const string SectionName = "Telegram";

    [Required]
    public required string AdminUsername { get; init; }

    [Required]
    public required string WebhookSecretToken { get; init; }

    [Required]
    [Range(0, int.MaxValue, MinimumIsExclusive = true)]
    public int WebhookRegistrationPeriodInMinutes { get; init; }
}
