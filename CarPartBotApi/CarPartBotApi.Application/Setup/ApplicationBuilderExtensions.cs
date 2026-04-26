using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.Background.BotMetadataUpdate;
using CarPartBotApi.Application.Background.TelegramWebhookProcessing;
using CarPartBotApi.Application.Background.WebhookRegistration;
using CarPartBotApi.Application.CommandExecutionPipeline;
using CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;
using CarPartBotApi.Application.CommandExecutionPipeline.Handlers;
using CarPartBotApi.Application.CommandExecutionPipeline.Handlers.Admin;
using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Application.Interfaces;
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
        services
            .AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>()
            .AddScoped<ITelegramContextAccessor, TelegramContextAccessor>();

        // Services.
        services.AddScoped<ITelegramService, TelegramService>();

        // Command handling.
        services.AddScoped<ICommandExecutionPipelineBuilder, CommandExecutionPipelineBuilder>();

        services
            .AddScoped<ICommandHandler, StartCommandHandler>()
            .AddScoped<ICommandHandler, GetUsersCommandHandler>()
            .AddScoped<ICommandHandler, HelpCommandHandler>()
            .AddScoped<ICommandHandler, ListCarsCommandHandler>()
            .AddScoped<ICommandHandler, AddCarCommandHandler>();

        services.AddScoped<RemoveCarCommandHandler>();
        services.AddScoped<ICommandHandler>(sp => sp.GetRequiredService<RemoveCarCommandHandler>());
        services.AddScoped<ICallbackableHandler>(sp => sp.GetRequiredService<RemoveCarCommandHandler>());

        services.AddScoped<IFailureHandler, FailureHandler>();

        // Background.
        services.AddHostedService<TelegramWebhookRegistrationWorker>();
        services.AddHostedService<BotMetadataUpdateWorker>();

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
