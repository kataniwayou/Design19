# Implementation Guidelines

This document provides detailed implementation guidelines for the FlowOrchestrator system components, focusing on common patterns, infrastructure integration, and best practices.

## 1. Common Implementation Patterns

### 1.1 Message-Based Communication

All components communicate through the message bus (MassTransit) using a consistent pattern:

```csharp
// Example from AbstractProcessorService
public virtual async Task Consume(ConsumeContext<ProcessCommand> context)
{
    _logger.LogInformation("Process command received: {CommandId}", context.Message.CommandId);

    try
    {
        // Validate input
        var validationResult = ValidateConfiguration(context.Message.Configuration);
        if (!validationResult.IsValid)
        {
            await PublishError(context, validationResult.Errors);
            return;
        }

        // Process data
        var result = Process(new ProcessParameters
        {
            InputData = context.Message.InputData,
            Configuration = context.Message.Configuration,
            ExecutionContext = context.Message.ExecutionContext
        }, context.Message.ExecutionContext);

        // Publish result
        await context.Publish(new ProcessCommandResult
        {
            CommandId = context.Message.CommandId,
            ProcessorId = ProcessorId,
            ProcessorVersion = Version,
            OutputData = result.TransformedData,
            Metadata = result.TransformationMetadata,
            ExecutionContext = context.Message.ExecutionContext
        });

        _logger.LogInformation("Process command completed: {CommandId}", context.Message.CommandId);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing command: {CommandId}", context.Message.CommandId);
        await PublishError(context, new[] { ex.Message });
    }
}
```

### 1.2 Observability Integration

All components integrate with OpenTelemetry for comprehensive observability:

```csharp
// Example from AbstractProcessorApplication
private void ConfigureOpenTelemetry(IServiceCollection services)
{
    services.AddOpenTelemetry()
        .WithTracing(builder => builder
            .AddSource(ProcessorName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(ProcessorName, serviceVersion: Version))
            .AddMassTransitInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options => options.Endpoint = new Uri(_configuration["Telemetry:OtlpEndpoint"])))
        .WithMetrics(builder => builder
            .AddMeter(ProcessorName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(ProcessorName, serviceVersion: Version))
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddOtlpExporter(options => options.Endpoint = new Uri(_configuration["Telemetry:OtlpEndpoint"])));

    services.AddLogging(builder => builder
        .AddOpenTelemetry(options => options
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(ProcessorName, serviceVersion: Version))
            .AddOtlpExporter(options => options.Endpoint = new Uri(_configuration["Telemetry:OtlpEndpoint"]))));
}
```

### 1.3 Configuration Management

All components use a consistent configuration approach:

```csharp
// Example from AbstractProcessorApplication
private void ConfigureServices(IServiceCollection services)
{
    // Add configuration
    services.Configure<ProcessorOptions>(_configuration.GetSection("Processor"));

    // Add core services
    services.AddSingleton<IProcessorService, TProcessor>();
    services.AddSingleton<IRegistrationPublisher, RegistrationPublisher>();

    // Add infrastructure services
    ConfigureMassTransit(services);
    ConfigureOpenTelemetry(services);

    // Add component-specific services
    ConfigureComponentServices(services);
}
```

### 1.4 Error Handling

All components implement consistent error handling:

```csharp
// Example from AbstractProcessorService
protected virtual async Task PublishError(ConsumeContext<ProcessCommand> context, IEnumerable<string> errors)
{
    await context.Publish(new ProcessCommandError
    {
        CommandId = context.Message.CommandId,
        ProcessorId = ProcessorId,
        ProcessorVersion = Version,
        Errors = errors.ToList(),
        ErrorTimestamp = DateTime.UtcNow,
        ExecutionContext = context.Message.ExecutionContext
    });

    _logger.LogError("Process command failed: {CommandId}, Errors: {Errors}",
        context.Message.CommandId, string.Join("; ", errors));
}
```

## 2. Infrastructure Integration

### 2.1 MassTransit Integration

All components integrate with MassTransit for message-based communication:

```csharp
// Example from AbstractProcessorApplication
private void ConfigureMassTransit(IServiceCollection services)
{
    services.AddMassTransit(busConfig =>
    {
        // Register message consumers
        busConfig.AddConsumer<CommandConsumer>();

        busConfig.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(_configuration["RabbitMQ:Host"], _configuration["RabbitMQ:VirtualHost"], h =>
            {
                h.Username(_configuration["RabbitMQ:Username"]);
                h.Password(_configuration["RabbitMQ:Password"]);
            });

            // Configure endpoint for receiving commands
            cfg.ReceiveEndpoint($"{ProcessorId}-commands", e =>
            {
                e.ConfigureConsumer<CommandConsumer>(context);
            });
        });
    });
}
### 2.2 MongoDB Integration (Entity Managers)

Entity Managers integrate with MongoDB for persistent storage:

```csharp
// Example from AbstractEntityManagerApplication
private void ConfigureMongoDB(IServiceCollection services)
{
    services.Configure<MongoDbSettings>(_configuration.GetSection("MongoDB"));

    services.AddSingleton<IMongoClient>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
        return new MongoClient(settings.ConnectionString);
    });

    services.AddSingleton<IMongoDbContext, MongoDbContext>();
    services.AddSingleton<IEntityRepository, EntityRepository>();
}

// Example MongoDbContext implementation
public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}
### 2.3 OpenTelemetry Integration

All components integrate with OpenTelemetry for comprehensive observability:

```csharp
// Example from AbstractEntityManagerApplication
private void ConfigureOpenTelemetry(IServiceCollection services)
{
    services.AddOpenTelemetry()
        .WithTracing(builder => builder
            .AddSource(EntityManagerName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(EntityManagerName, serviceVersion: Version))
            .AddAspNetCoreInstrumentation()
            .AddMassTransitInstrumentation()
            .AddMongoDBInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options => options.Endpoint = new Uri(_configuration["Telemetry:OtlpEndpoint"])))
        .WithMetrics(builder => builder
            .AddMeter(EntityManagerName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(EntityManagerName, serviceVersion: Version))
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddOtlpExporter(options => options.Endpoint = new Uri(_configuration["Telemetry:OtlpEndpoint"])));

    services.AddLogging(builder => builder
        .AddOpenTelemetry(options => options
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(EntityManagerName, serviceVersion: Version))
            .AddOtlpExporter(options => options.Endpoint = new Uri(_configuration["Telemetry:OtlpEndpoint"]))));
}
```

## 3. Implementation Best Practices

### 3.1 Concrete Implementation Guidelines

When implementing concrete components, follow these guidelines:

1. **Focus on Core Logic**: Concrete implementations should focus only on their specific logic, not infrastructure concerns
2. **Override Abstract Properties**: Implement all abstract properties with appropriate values
3. **Implement Core Methods**: Override abstract methods with component-specific implementations
4. **Avoid Infrastructure Changes**: Don't modify infrastructure code in concrete implementations
5. **Follow Naming Conventions**: Use consistent naming patterns for all components
6. **Document Capabilities**: Clearly document component capabilities and schemas
7. **Implement Validation**: Include thorough validation of all inputs
8. **Handle Errors Gracefully**: Implement proper error handling and recovery
9. **Include Unit Tests**: Create comprehensive unit tests for all implementations
10. **Document Configuration**: Clearly document all configuration options

### 3.2 Entity Manager Implementation Guidelines

When implementing Entity Managers, follow these additional guidelines:

1. **Standardized CRUD Controllers**: Implement consistent CRUD operations for all entity types
2. **Validation Logic**: Implement thorough validation of entity relationships and compatibility
3. **Authentication/Authorization**: Implement proper authentication and authorization for all endpoints
4. **API Documentation**: Use Swagger/OpenAPI to document all API endpoints
5. **Consistent Response Formats**: Use consistent response formats for all API endpoints
6. **Proper Error Handling**: Implement proper error handling and return appropriate HTTP status codes
7. **Pagination Support**: Implement pagination for all collection endpoints
8. **Filtering Support**: Implement filtering for all collection endpoints
9. **Versioning Support**: Implement API versioning for all endpoints
10. **Caching Strategy**: Implement appropriate caching for frequently accessed data

### 3.3 Testing Guidelines

All components should have comprehensive test coverage:

1. **Unit Tests**: Test individual methods and classes in isolation
2. **Integration Tests**: Test interactions between components
3. **System Tests**: Test end-to-end flows
4. **Performance Tests**: Test performance under load
5. **Resilience Tests**: Test recovery from failures
6. **Schema Validation Tests**: Test schema compatibility
7. **Configuration Validation Tests**: Test configuration validation
8. **API Contract Tests**: Test API contracts
9. **Message Contract Tests**: Test message contracts
10. **Security Tests**: Test authentication and authorization

## 4. Configuration Reference

### 4.1 Common Configuration Settings

All components use a consistent configuration structure:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "RabbitMQ": {
    "Host": "rabbitmq.example.com",
    "VirtualHost": "/",
    "Username": "user",
    "Password": "password"
  },
  "Telemetry": {
    "OtlpEndpoint": "http://otel-collector:4317"
  }
}
```

### 4.2 Entity Manager Configuration

Entity Managers have additional configuration settings:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://mongodb.example.com:27017",
    "DatabaseName": "FlowOrchestrator"
  },
  "Authentication": {
    "Authority": "https://identity.example.com",
    "Audience": "flowOrchestrator.api"
  }
}
```