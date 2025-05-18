using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using Microsoft.AspNetCore.Mvc;
using ILogger = FlowOrchestrator.Common.Logging.ILogger;

namespace FlowOrchestrator.EntityManagerBase.Controllers;

/// <summary>
/// Abstract base controller for entity management.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TValidator">The type of validator.</typeparam>
[ApiController]
[Route("api/[controller]")]
public abstract class AbstractEntityController<TEntity, TValidator> : ControllerBase
    where TEntity : AbstractEntity
    where TValidator : IValidator<TEntity>
{
    private readonly IMongoRepository<TEntity> _repository;
    private readonly TValidator _validator;
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractEntityController{TEntity, TValidator}"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    protected AbstractEntityController(
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
    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TEntity>>> GetAll()
    {
        using var span = _telemetryProvider.CreateSpan($"{typeof(TEntity).Name}.GetAll");

        try
        {
            _logger.Info($"Getting all {typeof(TEntity).Name} entities");
            var entities = await _repository.GetAllAsync();
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get all {typeof(TEntity).Name} entities", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The entity.</returns>
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TEntity>> GetById(string id)
    {
        using var span = _telemetryProvider.CreateSpan($"{typeof(TEntity).Name}.GetById");
        span.SetAttribute("entity.id", id);

        try
        {
            _logger.Info($"Getting {typeof(TEntity).Name} entity with ID: {id}");
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.Warn($"{typeof(TEntity).Name} entity with ID: {id} not found");
                return NotFound();
            }

            return Ok(entity);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {typeof(TEntity).Name} entity with ID: {id}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity.</returns>
    [HttpPost]
    public virtual async Task<ActionResult<TEntity>> Create(TEntity entity)
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
                return BadRequest(validationResult.Errors);
            }

            entity.CreatedTimestamp = DateTime.UtcNow;
            entity.LastModifiedTimestamp = DateTime.UtcNow;

            var createdEntity = await _repository.AddAsync(entity);
            _logger.Info($"Created {typeof(TEntity).Name} entity with ID: {createdEntity.Id}");

            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, createdEntity);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to create {typeof(TEntity).Name} entity", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="entity">The entity to update.</param>
    /// <returns>No content.</returns>
    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Update(string id, TEntity entity)
    {
        using var span = _telemetryProvider.CreateSpan($"{typeof(TEntity).Name}.Update");
        span.SetAttribute("entity.id", id);

        try
        {
            if (id != entity.Id)
            {
                _logger.Warn($"ID mismatch for {typeof(TEntity).Name} entity: {id} != {entity.Id}");
                return BadRequest("ID mismatch");
            }

            _logger.Info($"Updating {typeof(TEntity).Name} entity with ID: {id}");

            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
            {
                _logger.Warn($"{typeof(TEntity).Name} entity with ID: {id} not found");
                return NotFound();
            }

            var validationResult = _validator.Validate(entity);
            if (!validationResult.IsValid)
            {
                _logger.Warn($"Validation failed for {typeof(TEntity).Name} entity with ID: {id}");
                return BadRequest(validationResult.Errors);
            }

            entity.CreatedTimestamp = existingEntity.CreatedTimestamp;
            entity.LastModifiedTimestamp = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            _logger.Info($"Updated {typeof(TEntity).Name} entity with ID: {id}");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to update {typeof(TEntity).Name} entity with ID: {id}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(string id)
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
                return NotFound();
            }

            var result = await _repository.DeleteAsync(id);
            if (!result)
            {
                _logger.Warn($"Failed to delete {typeof(TEntity).Name} entity with ID: {id}");
                return StatusCode(500, "Failed to delete entity");
            }

            _logger.Info($"Deleted {typeof(TEntity).Name} entity with ID: {id}");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to delete {typeof(TEntity).Name} entity with ID: {id}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
