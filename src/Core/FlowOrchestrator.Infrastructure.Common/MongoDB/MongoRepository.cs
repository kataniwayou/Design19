using FlowOrchestrator.Abstractions.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace FlowOrchestrator.Infrastructure.Common.MongoDB;

/// <summary>
/// MongoDB repository implementation.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : AbstractEntity
{
    private readonly IMongoCollection<TEntity> _collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="database">The MongoDB database.</param>
    /// <param name="collectionName">The name of the collection.</param>
    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<TEntity>(collectionName);
    }

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <returns>All entities.</returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    /// <summary>
    /// Gets entities by a filter.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <returns>The filtered entities.</returns>
    public async Task<IEnumerable<TEntity>> GetByFilterAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The entity, or null if not found.</returns>
    public async Task<TEntity?> GetByIdAsync(string id)
    {
        return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Adds an entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The updated entity.</returns>
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.LastModifiedTimestamp = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
        return entity;
    }

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>True if the entity was deleted, false otherwise.</returns>
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(e => e.Id == id);
        return result.DeletedCount > 0;
    }

    /// <summary>
    /// Checks if an entity exists.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>True if the entity exists, false otherwise.</returns>
    public async Task<bool> ExistsAsync(string id)
    {
        return await _collection.Find(e => e.Id == id).AnyAsync();
    }

    /// <summary>
    /// Counts entities by a filter.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <returns>The count of entities.</returns>
    public async Task<long> CountAsync(Expression<Func<TEntity, bool>>? filter = null)
    {
        return filter == null
            ? await _collection.CountDocumentsAsync(_ => true)
            : await _collection.CountDocumentsAsync(filter);
    }
}
