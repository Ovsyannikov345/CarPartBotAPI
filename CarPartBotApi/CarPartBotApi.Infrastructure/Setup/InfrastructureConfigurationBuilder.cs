namespace CarPartBotApi.Infrastructure.Setup;

public sealed class InfrastructureConfigurationBuilder
{
    private string? _webhookEndpointUrl;

    internal string? WebhookEndpointUrl => _webhookEndpointUrl;

    public InfrastructureConfigurationBuilder WithTelegramWebhookEndpoint(string? webhookEndpointUrl)
    {
        _webhookEndpointUrl = webhookEndpointUrl;

        return this;
    }
}
