using Serilog;

namespace CarPartBotApi.Api.Setup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        Log.Information("Configuring CarPartBotApi.API services...");

        services.AddControllers();
        services.AddOpenApi();

        Log.Information("CarPartBotApi.API services configured.");

        return services;
    }
}
