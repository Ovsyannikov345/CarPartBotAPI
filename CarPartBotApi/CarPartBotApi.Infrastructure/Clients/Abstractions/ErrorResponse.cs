namespace CarPartBotApi.Infrastructure.Clients.Abstractions;

public abstract record ErrorResponse
{
    public abstract string GetErrorMessage();
}
