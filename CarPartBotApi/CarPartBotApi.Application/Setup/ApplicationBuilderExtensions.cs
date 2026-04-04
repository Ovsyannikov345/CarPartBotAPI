using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Background.TelegramWebhookProcessing;
using CarPartBotApi.Application.Background.WebhookRegistration;
using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Application.Handlers;
using CarPartBotApi.Application.Handlers.Admin;
using CarPartBotApi.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CarPartBotApi.Application.Setup;

public static class ApplicationBuilderExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Information("Configuring CarPartBotApi.Application services...");

        // Configuration.
        services
            .AddOptions<TelegramSettings>()
            .Bind(configuration.GetSection(TelegramSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Accessors.
        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

        // Services.
        services.AddScoped<ITelegramService, TelegramService>();

        // Command handlers.
        services
            .AddScoped<ICommandHandler, StartCommandHandler>()
            .AddScoped<ICommandHandler, HelpCommandHandler>();

        // Admin command handlers.
        services
            .AddScoped<ICommandHandler, GetUsersCommandHandler>();

        // Background.
        services.AddHostedService<TelegramWebhookRegistrationWorker>();

        services
            .AddScoped<ITelegramWebhookEventDispatcher, TelegramWebhookEventDispatcher>()
            .AddSingleton<TelegramWebhookChannel>()
            .AddSingleton<ITelegramWebhookEventWriter>((sp) => sp.GetRequiredService<TelegramWebhookChannel>())
            .AddSingleton<ITelegramWebhookEventReader>((sp) => sp.GetRequiredService<TelegramWebhookChannel>())
            .AddHostedService<TelegramWebhookEventWorker>();

        Log.Information("CarPartBotApi.Application services configured.");

        return services;
    }
}
