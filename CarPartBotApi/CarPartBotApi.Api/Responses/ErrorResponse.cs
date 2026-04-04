namespace CarPartBotApi.Api.Responses;

public sealed record ErrorResponse
{
    public required int StatusCode { get; init; }

    public required string ErrorMessage { get; init; }
}
