using MongoDB.Driver;

namespace FlowOrchestrator.Infrastructure.Common.MongoDB;

/// <summary>
/// MongoDB database context.
/// </summary>
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbContext"/> class.
    /// </summary>
    /// <param name="connectionString">The MongoDB connection string.</param>
    /// <param name="databaseName">The name of the database.</param>
    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    /// <summary>
    /// Gets the MongoDB database.
    /// </summary>
    public IMongoDatabase Database => _database;

    /// <summary>
    /// Gets a repository for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="collectionName">The name of the collection.</param>
    /// <returns>The repository.</returns>
    public IMongoRepository<TEntity> GetRepository<TEntity>(string collectionName) where TEntity : Abstractions.Entities.AbstractEntity
    {
        return new MongoRepository<TEntity>(_database, collectionName);
    }
}
