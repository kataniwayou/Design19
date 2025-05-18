using FlowOrchestrator.Infrastructure.Common.Messaging;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit.RabbitMqTransport;

namespace FlowOrchestrator.EntityManagerBase.Infrastructure.MassTransit;

/// <summary>
/// Extension methods for MassTransit integration.
/// </summary>
public static class MassTransitExtensions
{
    /// <summary>
    /// Adds MassTransit services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configureBus">The bus configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMassTransitIntegration(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureBus = null)
    {
        services.AddMassTransit(busConfig =>
        {
            // Allow additional configuration
            configureBus?.Invoke(busConfig);

            // Configure RabbitMQ
            busConfig.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var virtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
                var username = configuration["RabbitMQ:Username"] ?? "guest";
                var password = configuration["RabbitMQ:Password"] ?? "guest";

                cfg.Host(host, virtualHost, h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddSingleton<IMessageBus, MassTransitMessageBus>();

        return services;
    }
}
