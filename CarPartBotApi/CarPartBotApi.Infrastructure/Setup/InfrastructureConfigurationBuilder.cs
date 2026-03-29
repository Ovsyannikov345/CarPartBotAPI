using Microsoft.Extensions.Configuration;

namespace CarPartBotApi.Infrastructure.Setup;

public sealed class InfrastructureConfigurationBuilder
{
    private IConfiguration? _configuration;

    private string? _webhookEndpointUrl;

    internal IConfiguration? Configuration => _configuration;

    internal string? WebhookEndpointUrl => _webhookEndpointUrl;

    public InfrastructureConfigurationBuilder WithConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;

        return this;
    }

    public InfrastructureConfigurationBuilder WithTelegramWebhookEndpoint(string? webhookEndpointUrl)
    {
        _webhookEndpointUrl = webhookEndpointUrl;

        return this;
    }
}
