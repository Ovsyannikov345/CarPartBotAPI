using CarPartBotApi.Api.Setup;
using CarPartBotApi.Application.Setup;
using CarPartBotApi.Infrastructure.Setup;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddApi()
    .AddApplication(configuration)
    .AddInfrastructure(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs", options =>
    {
        options.WithTitle("API Reference");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
