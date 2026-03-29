using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace CarPartBotApi.Application.Configuration;

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
}

public sealed record ConnectionStrings
{
    public const string SectionName = "ConnectionStrings";

    [Required]
    public required string Postgres { get; init; }
}
