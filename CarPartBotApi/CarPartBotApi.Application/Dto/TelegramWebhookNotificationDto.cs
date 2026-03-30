namespace CarPartBotApi.Application.Dto;

public sealed record TelegramWebhookNotificationDto
{
    public long UpdateId { get; init; }

    public MessageDto? Message { get; init; }
}

public sealed record MessageDto
{
    public long MessageId { get; init; }

    public UserDto? From { get; init; }

    public required ChatDto Chat { get; init; }

    public string? Text { get; init; }

    public List<MessageEntityDto>? Entities { get; init; }
}

public sealed record UserDto
{
    public long Id { get; init; }

    public required string FirstName { get; init; }

    public string? LastName { get; init; }

    public string? Username { get; init; }

    public string? LanguageCode { get; init; }
}

public sealed record ChatDto
{
    public required long Id { get; init; }

    public required string Type { get; init; }
}

public sealed record MessageEntityDto
{
    public required string Type { get; init; }

    public required int Offset { get; init; }

    public required int Length { get; init; }
}
