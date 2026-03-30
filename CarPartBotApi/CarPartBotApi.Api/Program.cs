using CarPartBotApi.Api.Constants;
using CarPartBotApi.Api.Setup;
using CarPartBotApi.Application.Setup;
using CarPartBotApi.Infrastructure.Database;
using CarPartBotApi.Infrastructure.Setup;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Building and starting the application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    var configuration = builder.Configuration;

    builder.Services
        .AddApi()
        .AddApplication(configuration)
        .AddInfrastructure(configuration, options => options
            .WithTelegramWebhookEndpoint($"{WebhookConstants.WebhookControllerPath}/{WebhookConstants.TelegramWebhookEndpointPath}"));

    Log.Information("Services configured successfully.");

    Log.Information("Configuring request pipeline...");

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference("/docs", options =>
        {
            options.WithTitle("API Reference");
        });
    }

    // TODO add exception middleware.

    // TODO remove and provide custom solution.
    app.UseSerilogRequestLogging();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Request pipeline configured successfully.");

    Log.Information("Starting the application...");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
