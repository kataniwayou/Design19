using FlowOrchestrator.AnalyticsEngine;
using FlowOrchestrator.AnalyticsEngine.Services;
using FlowOrchestrator.Common.Configuration;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Messaging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using MassTransit;
using MongoDB.Driver;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

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
    new FlowOrchestrator.Infrastructure.Common.Telemetry.OpenTelemetryProvider("AnalyticsEngine"));

// Configure OpenTelemetry
builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("AnalyticsEngine"));
});

// Add application services
builder.Services.AddSingleton<AnalyticsService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
