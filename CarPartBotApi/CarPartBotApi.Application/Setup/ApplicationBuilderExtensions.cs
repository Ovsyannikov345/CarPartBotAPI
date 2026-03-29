using CarPartBotApi.Application.Background;
using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Application.Services;
using CarPartBotApi.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarPartBotApi.Application.Setup;

public static class ApplicationBuilderExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration.
        services
            .AddOptions<TelegramSettings>()
            .Bind(configuration.GetSection(TelegramSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Services.
        services.AddScoped<ITelegramService, TelegramService>();

        // Background.
        services.AddHostedService<TelegramWebhookRegistrationWorker>();

        return services;
    }
}
