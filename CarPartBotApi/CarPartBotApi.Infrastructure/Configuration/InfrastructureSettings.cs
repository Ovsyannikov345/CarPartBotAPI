using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace CarPartBotApi.Infrastructure.Configuration;

public sealed record InfrastructureSettings
{
    public const string SectionName = "Infrastructure";

    [Required]
    [Url]
    public required string ApiBaseUrl { get; init; }

    public string? WebhookEndpointUrl { get; init; }

    [Required]
    [ValidateObjectMembers]
    public required ConnectionStrings ConnectionStrings { get; init; }

    [Required]
    [ValidateObjectMembers]
    public required TelegramSettings Telegram { get; init; }
}

public sealed record ConnectionStrings
{
    [Required]
    public required string Posgres { get; init; }
}

public sealed record TelegramSettings
{
    [Required]
    [Url]
    public required string ApiBaseUrl { get; init; }

    [Required]
    public required string BotToken { get; init; }
}
