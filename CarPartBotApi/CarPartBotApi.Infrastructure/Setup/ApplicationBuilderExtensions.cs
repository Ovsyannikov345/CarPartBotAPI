using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Domain.Interfaces.Clients;
using CarPartBotApi.Domain.Interfaces.Data;
using CarPartBotApi.Infrastructure.Clients.Telegram;
using CarPartBotApi.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace CarPartBotApi.Infrastructure.Setup;

public static class ApplicationBuilderExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Action<InfrastructureConfigurationBuilder> options)
    {
        Log.Information("Configuring CarPartBotApi.Infrastructure services...");

        AddConfiguration(services, configuration, options);

        // Database.
        services.AddDbContext<IApplicationDbContext, ApplicationDbContext>();

        // Http clients.
        services.AddHttpClient<ITelegramClient, TelegramClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<TelegramSettings>>();

            client.BaseAddress = new Uri($"{options.Value.ApiBaseUrl}/bot{options.Value.BotToken}/", UriKind.Absolute);
        });
        // TODO add logging handler.

        Log.Information("CarPartBotApi.Infrastructure services configured.");

        return services;
    }

    private static void AddConfiguration(IServiceCollection services, IConfiguration configuration, Action<InfrastructureConfigurationBuilder> options)
    {
        var configBuilder = new InfrastructureConfigurationBuilder();
        options(configBuilder);

        var infrastructureConfiguration = configuration.GetSection(InfrastructureSettings.SectionName);

        if (configBuilder.WebhookEndpointUrl is not null)
        {
            if (!Uri.TryCreate(configBuilder.WebhookEndpointUrl, UriKind.Relative, out _))
            {
                throw new ArgumentException("Failed to setup CarPartBotApi.Infrastructure services: WebhookEndpointUrl must be a valid relative URL if provided.");
            }

            infrastructureConfiguration["WebhookEndpointUrl"] = configBuilder.WebhookEndpointUrl;
        }

        services
            .AddOptions<InfrastructureSettings>()
            .Bind(infrastructureConfiguration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<TelegramSettings>()
            .Bind(configuration.GetSection(TelegramSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
