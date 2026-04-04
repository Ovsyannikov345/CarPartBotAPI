using CarPartBotApi.Application.Background;
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

        Log.Information("CarPartBotApi.Application services configured.");

        return services;
    }
}
