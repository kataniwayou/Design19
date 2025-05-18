using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using ILogger = FlowOrchestrator.Common.Logging.ILogger;

namespace FlowOrchestrator.EntityManagerBase.Services;

/// <summary>
/// Abstract base service for entity management.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TValidator">The type of validator.</typeparam>
public abstract class AbstractEntityService<TEntity, TValidator>
    where TEntity : AbstractEntity
    where TValidator : IValidator<TEntity>
{
    private readonly IMongoRepository<TEntity> _repository;
    private readonly TValidator _validator;
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractEntityService{TEntity, TValidator}"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    protected AbstractEntityService(
        IMongoRepository<TEntity> repository,
        TValidator validator,
        ILogger logger,
        ITelemetryProvider telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
    }

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <returns>All entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        using var span = _telemetryProvider.CreateSpan($"{typeof(TEntity).Name}.GetAll");

        try
        {
            _logger.Info($"Getting all {typeof(TEntity).Name} entities");
            return await _repository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get all {typeof(TEntity).Name} entities", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The entity.</returns>
    public virtual async Task<TEntity?> GetByIdAsync(string id)
    {
        using var span = _telemetryProvider.CreateSpan($"{typeof(TEntity).Name}.GetById");
        span.SetAttribute("entity.id", id);

        try
        {
            _logger.Info($"Getting {typeof(TEntity).Name} entity with ID: {id}");
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {typeof(TEntity).Name} entity with ID: {id}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity.</returns>
    public virtual async Task<(TEntity Entity, ValidationResult ValidationResult)> CreateAsync(TEntity entity)
    {
        using var span = _telemetryProvider.CreateSpan($"{typeof(TEntity).Name}.Create");
        span.SetAttribute("entity.id", entity.Id);

        try
        {
            _logger.Info($"Creating {typeof(TEntity).Name} entity");

            var validationResult = _validator.Validate(entity);
            if (!validationResult.IsValid)
            {
                _logger.Warn($"Validation failed for {typeof(TEntity).Name} entity");
                return (entity, validationResult);
            }

            entity.CreatedTimestamp = DateTime.UtcNow;
            entity.LastModifiedTimestamp = DateTime.UtcNow;

            var createdEntity = await _repository.AddAsync(entity);
            _logger.Info($"Created {typeof(TEntity).Name} entity with ID: {createdEntity.Id}");

            return (createdEntity, validationResult);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to create {typeof(TEntity).Name} entity", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The validation result.</returns>
    public virtual async Task<(bool Success, ValidationResult ValidationResult)> UpdateAsync(string id, TEntity entity)
    {
        using var span = _telemetryProvider.CreateSpan($"{typeof(TEntity).Name}.Update");
        span.SetAttribute("entity.id", id);

        try
        {
            if (id != entity.Id)
            {
                _logger.Warn($"ID mismatch for {typeof(TEntity).Name} entity: {id} != {entity.Id}");
                var idMismatchResult = new ValidationResult();
                idMismatchResult.AddError("Id", "ID mismatch");
                return (false, idMismatchResult);
            }

            _logger.Info($"Updating {typeof(TEntity).Name} entity with ID: {id}");

            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
            {
                _logger.Warn($"{typeof(TEntity).Name} entity with ID: {id} not found");
                var notFoundResult = new ValidationResult();
                notFoundResult.AddError("Id", "Entity not found");
                return (false, notFoundResult);
            }

            var validationResult = _validator.Validate(entity);
            if (!validationResult.IsValid)
            {
                _logger.Warn($"Validation failed for {typeof(TEntity).Name} entity with ID: {id}");
                return (false, validationResult);
            }

            entity.CreatedTimestamp = existingEntity.CreatedTimestamp;
            entity.LastModifiedTimestamp = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            _logger.Info($"Updated {typeof(TEntity).Name} entity with ID: {id}");

            return (true, validationResult);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to update {typeof(TEntity).Name} entity with ID: {id}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>True if the entity was deleted, false otherwise.</returns>
    public virtual async Task<bool> DeleteAsync(string id)
    {
        using var span = _telemetryProvider.CreateSpan($"{typeof(TEntity).Name}.Delete");
        span.SetAttribute("entity.id", id);

        try
        {
            _logger.Info($"Deleting {typeof(TEntity).Name} entity with ID: {id}");

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.Warn($"{typeof(TEntity).Name} entity with ID: {id} not found");
                return false;
            }

            var result = await _repository.DeleteAsync(id);
            if (result)
            {
                _logger.Info($"Deleted {typeof(TEntity).Name} entity with ID: {id}");
            }
            else
            {
                _logger.Warn($"Failed to delete {typeof(TEntity).Name} entity with ID: {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to delete {typeof(TEntity).Name} entity with ID: {id}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Checks if an entity exists.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>True if the entity exists, false otherwise.</returns>
    public virtual async Task<bool> ExistsAsync(string id)
    {
        using var span = _telemetryProvider.CreateSpan($"{typeof(TEntity).Name}.Exists");
        span.SetAttribute("entity.id", id);

        try
        {
            _logger.Info($"Checking if {typeof(TEntity).Name} entity with ID: {id} exists");
            return await _repository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to check if {typeof(TEntity).Name} entity with ID: {id} exists", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }
}
