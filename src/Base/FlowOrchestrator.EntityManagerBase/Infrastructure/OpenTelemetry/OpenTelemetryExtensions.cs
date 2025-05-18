using FlowOrchestrator.Infrastructure.Common.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;

namespace FlowOrchestrator.EntityManagerBase.Infrastructure.OpenTelemetry;

/// <summary>
/// Extension methods for OpenTelemetry integration.
/// </summary>
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds OpenTelemetry services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="serviceName">The service name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddOpenTelemetryIntegration(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);
        var otlpEndpoint = configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317";

        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddSource(serviceName)
                    .SetResourceBuilder(resourceBuilder)
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                builder.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint);
                });
            })
            .WithMetrics(builder =>
            {
                builder
                    .AddMeter(serviceName);

                // Note: Metrics exporter configuration is different from tracing
                // We're not setting resource builder or adding OTLP exporter for metrics
                // as it requires additional configuration
            });

        // Add OpenTelemetry logging via Microsoft.Extensions.Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole(); // Keep console logging for development

            // Note: OpenTelemetry logging integration requires additional packages
            // We'll use our custom OpenTelemetryLogger implementation instead
        });

        services.AddSingleton<ITelemetryProvider>(provider => new OpenTelemetryProvider(serviceName));

        return services;
    }
}
