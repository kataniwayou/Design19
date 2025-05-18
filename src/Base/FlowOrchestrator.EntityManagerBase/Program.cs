using FlowOrchestrator.Common.Configuration;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.EntityManagerBase.Infrastructure.Configuration;
using FlowOrchestrator.EntityManagerBase.Infrastructure.Logging;
using FlowOrchestrator.EntityManagerBase.Infrastructure.MassTransit;
using FlowOrchestrator.EntityManagerBase.Infrastructure.MongoDB;
using FlowOrchestrator.EntityManagerBase.Infrastructure.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Add infrastructure services
builder.Services.AddSingleton<FlowOrchestrator.Common.Logging.ILogger, FlowOrchestrator.EntityManagerBase.Infrastructure.Logging.ConsoleLogger>();
builder.Services.AddSingleton<FlowOrchestrator.Common.Configuration.IConfigurationProvider, FlowOrchestrator.EntityManagerBase.Infrastructure.Configuration.AppSettingsConfigurationProvider>();

// Add MongoDB
builder.Services.AddMongoDb(builder.Configuration);

// Add MassTransit
builder.Services.AddMassTransitIntegration(builder.Configuration);

// Add OpenTelemetry
builder.Services.AddOpenTelemetryIntegration(
    builder.Configuration,
    "FlowOrchestrator.EntityManager");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


