using FlowOrchestrator.Common.Configuration;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Messaging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.MonitoringFramework.Services;
using MassTransit;
using MongoDB.Driver;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services
builder.Services.AddSingleton<FlowOrchestrator.Common.Logging.ILogger>(provider =>
    new FlowOrchestrator.EntityManagerBase.Infrastructure.Logging.ConsoleLogger());

builder.Services.AddSingleton<FlowOrchestrator.Common.Configuration.IConfigurationProvider>(provider =>
    new FlowOrchestrator.EntityManagerBase.Infrastructure.Configuration.AppSettingsConfigurationProvider(builder.Configuration));

// Add MongoDB
builder.Services.AddSingleton<IMongoClient>(provider =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDB")));

// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<IMessageBus, MassTransitMessageBus>();

// Add OpenTelemetry
builder.Services.AddSingleton<ITelemetryProvider>(provider =>
    new FlowOrchestrator.Infrastructure.Common.Telemetry.OpenTelemetryProvider("MonitoringFramework"));

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource("MonitoringFramework")
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MonitoringFramework"))
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"] ?? "http://localhost:4317");
            });
    });

// Add application services
builder.Services.AddSingleton<MonitoringService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Initialize services
var monitoringService = app.Services.GetRequiredService<MonitoringService>();
await monitoringService.InitializeAsync();

app.Run();
