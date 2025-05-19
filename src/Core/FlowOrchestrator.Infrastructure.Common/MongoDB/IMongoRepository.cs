using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;
using System.Linq.Expressions;

namespace FlowOrchestrator.Infrastructure.Common.MongoDB;

/// <summary>
/// Defines the contract for a MongoDB repository.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public interface IMongoRepository<TEntity> : IRepository<TEntity> where TEntity : AbstractEntity
{
    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <returns>All entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// Gets entities by a filter.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <returns>The filtered entities.</returns>
    Task<IEnumerable<TEntity>> GetByFilterAsync(Expression<Func<TEntity, bool>> filter);

    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The entity, or null if not found.</returns>
    Task<TEntity?> GetByIdAsync(string id);

    /// <summary>
    /// Adds an entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Creates an entity.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity.</returns>
    Task<TEntity> CreateAsync(TEntity entity);

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The updated entity.</returns>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>True if the entity was deleted, false otherwise.</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// Checks if an entity exists.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>True if the entity exists, false otherwise.</returns>
    Task<bool> ExistsAsync(string id);

    /// <summary>
    /// Counts entities by a filter.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <returns>The count of entities.</returns>
    Task<long> CountAsync(Expression<Func<TEntity, bool>>? filter = null);
}
