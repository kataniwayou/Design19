using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Infrastructure.Logging;
using FlowOrchestrator.EntityManagerBase.Infrastructure.MassTransit;
using FlowOrchestrator.EntityManagerBase.Infrastructure.MongoDB;
using FlowOrchestrator.EntityManagerBase.Infrastructure.OpenTelemetry;
using FlowOrchestrator.ProcessorEntityManager.Services;
using FlowOrchestrator.ProcessorEntityManager.Validators;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using ILogger = FlowOrchestrator.Common.Logging.ILogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Add infrastructure services
builder.Services.AddSingleton<ITelemetryProvider>(provider =>
    new OpenTelemetryProvider("FlowOrchestrator.ProcessorEntityManager"));
builder.Services.AddSingleton<ILogger>(provider =>
    new OpenTelemetryLogger(
        provider.GetRequiredService<ITelemetryProvider>(),
        "FlowOrchestrator.ProcessorEntityManager"));

// Add MongoDB
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddMongoRepository<ProcessorEntity>("processors");

// Add MassTransit
builder.Services.AddMassTransitIntegration(builder.Configuration);

// Add OpenTelemetry
builder.Services.AddOpenTelemetryIntegration(
    builder.Configuration,
    "FlowOrchestrator.ProcessorEntityManager");

// Add application services
builder.Services.AddSingleton<IValidator<ProcessorEntity>, ProcessorValidator>();
builder.Services.AddSingleton<ProcessorService>();

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
