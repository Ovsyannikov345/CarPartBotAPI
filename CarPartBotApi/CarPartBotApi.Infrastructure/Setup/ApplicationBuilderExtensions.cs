using CarPartBotApi.Domain.Interfaces.Clients;
using CarPartBotApi.Domain.Interfaces.Data;
using CarPartBotApi.Infrastructure.Clients.Telegram;
using CarPartBotApi.Infrastructure.Configuration;
using CarPartBotApi.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CarPartBotApi.Infrastructure.Setup;

public static class ApplicationBuilderExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<InfrastructureConfigurationBuilder> options)
    {
        AddConfiguration(services, options);

        // Database.
        services.AddDbContext<IApplicationDbContext, ApplicationDbContext>();

        // Http clients.
        services.AddHttpClient<ITelegramClient, TelegramClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<InfrastructureSettings>>();

            var telegramSettings = options.Value.Telegram;

            client.BaseAddress = new Uri($"{telegramSettings.ApiBaseUrl}/bot{telegramSettings.BotToken}", UriKind.Absolute);
        });

        return services;
    }

    private static void AddConfiguration(IServiceCollection services, Action<InfrastructureConfigurationBuilder> options)
    {
        var configBuilder = new InfrastructureConfigurationBuilder();
        options(configBuilder);

        if (configBuilder.Configuration is null)
        {
            throw new InvalidOperationException("Failed to setup CarPartBotApi.Infrastructure services: configuration is not provided.");
        }

        if (configBuilder.WebhookEndpointUrl is not null)
        {
            if (!Uri.TryCreate(configBuilder.WebhookEndpointUrl, UriKind.Relative, out _))
            {
                throw new ArgumentException("Failed to setup CarPartBotApi.Infrastructure services: WebhookEndpointUrl must be a valid relative URL if provided.");
            }

            configBuilder.Configuration["WebhookEndpointUrl"] = configBuilder.WebhookEndpointUrl;
        }

        services
            .AddOptions<InfrastructureSettings>()
            .Bind(configBuilder.Configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
