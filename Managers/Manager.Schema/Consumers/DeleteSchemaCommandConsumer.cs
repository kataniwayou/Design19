﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Manager.Schema.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.MassTransit.Commands;
using Shared.MassTransit.Events;

namespace Manager.Schema.Consumers;

public class DeleteSchemaCommandConsumer : IConsumer<DeleteSchemaCommand>
{
    private readonly ISchemaEntityRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<DeleteSchemaCommandConsumer> _logger;

    public DeleteSchemaCommandConsumer(
        ISchemaEntityRepository repository,
        IPublishEndpoint publishEndpoint,
        ILogger<DeleteSchemaCommandConsumer> logger)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeleteSchemaCommand> context)
    {
        var stopwatch = Stopwatch.StartNew();
        var command = context.Message;

        _logger.LogInformation("Processing DeleteSchemaCommand. Id: {Id}, RequestedBy: {RequestedBy}",
            command.Id, command.RequestedBy);

        try
        {
            var existingEntity = await _repository.GetByIdAsync(command.Id);
            if (existingEntity == null)
            {
                _logger.LogWarning("Schema entity not found for deletion. Id: {Id}", command.Id);
                await context.RespondAsync(new DeleteSchemaCommandResponse
                {
                    Success = false,
                    Message = $"Schema entity with ID {command.Id} not found"
                });
                return;
            }

            var success = await _repository.DeleteAsync(command.Id);

            if (success)
            {
                await _publishEndpoint.Publish(new SchemaDeletedEvent
                {
                    Id = command.Id,
                    DeletedAt = DateTime.UtcNow,
                    DeletedBy = command.RequestedBy
                });

                stopwatch.Stop();
                _logger.LogInformation("Successfully processed DeleteSchemaCommand. Id: {Id}, Duration: {Duration}ms",
                    command.Id, stopwatch.ElapsedMilliseconds);

                await context.RespondAsync(new DeleteSchemaCommandResponse
                {
                    Success = true,
                    Message = "Schema entity deleted successfully"
                });
            }
            else
            {
                stopwatch.Stop();
                _logger.LogWarning("Failed to delete Schema entity. Id: {Id}, Duration: {Duration}ms",
                    command.Id, stopwatch.ElapsedMilliseconds);

                await context.RespondAsync(new DeleteSchemaCommandResponse
                {
                    Success = false,
                    Message = "Failed to delete Schema entity"
                });
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error processing DeleteSchemaCommand. Id: {Id}, Duration: {Duration}ms",
                command.Id, stopwatch.ElapsedMilliseconds);

            await context.RespondAsync(new DeleteSchemaCommandResponse
            {
                Success = false,
                Message = $"Failed to delete Schema entity: {ex.Message}"
            });
        }
    }
}

public class DeleteSchemaCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
