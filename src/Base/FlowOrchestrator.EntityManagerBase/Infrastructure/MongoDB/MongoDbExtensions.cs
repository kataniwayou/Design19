using FlowOrchestrator.Infrastructure.Common.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FlowOrchestrator.EntityManagerBase.Infrastructure.MongoDB;

/// <summary>
/// Extension methods for MongoDB integration.
/// </summary>
public static class MongoDbExtensions
{
    /// <summary>
    /// Adds MongoDB services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
        var databaseName = configuration["MongoDB:DatabaseName"] ?? "FlowOrchestrator";

        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
        services.AddSingleton<MongoDbContext>(provider => new MongoDbContext(connectionString, databaseName));

        return services;
    }

    /// <summary>
    /// Adds a MongoDB repository for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="collectionName">The name of the collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMongoRepository<TEntity>(this IServiceCollection services, string collectionName)
        where TEntity : FlowOrchestrator.Abstractions.Entities.AbstractEntity
    {
        services.AddSingleton<IMongoRepository<TEntity>>(provider =>
        {
            var context = provider.GetRequiredService<MongoDbContext>();
            return context.GetRepository<TEntity>(collectionName);
        });

        return services;
    }
}
