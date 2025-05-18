6. Service Management (Continued)
6.2.4 Flow Entity Manager
The Flow Entity Manager is responsible for managing Flow definitions in the system.

Implementation:

ASP.NET Core WebAPI application
MassTransit for message bus integration
MongoDB for persistent storage
OpenTelemetry for observability
Key Responsibilities:

Manages flow definitions and their versions
Validates flow structure and completeness
Verifies component compatibility within flows
Ensures proper branch configuration
Validates merge strategy configuration
Provides visualization capabilities for flows
Executes flow deployment operations
Tracks flow execution history
Manages flow version lifecycle
Key APIs:

GET /api/flows - List all flows
GET /api/flows/{id} - Get flow by ID
GET /api/flows/{id}/versions - Get all versions of a flow
POST /api/flows - Create new flow
PUT /api/flows/{id} - Update flow
DELETE /api/flows/{id} - Delete flow
POST /api/flows/{id}/deploy - Deploy flow to execution environment
GET /api/flows/{id}/history - Get flow execution history
Message Consumers:

FlowExecutionStatusConsumer - Tracks flow execution status updates
ComponentAvailabilityConsumer - Monitors component availability for flows
Storage:

MongoDB collection: Flows
MongoDB collection: FlowVersions
MongoDB collection: FlowExecutionHistory
Indexing: FlowId, Version, Status
6.3 Message Contracts
The FlowOrchestrator system uses standardized message contracts for communication between components and Entity Managers.

6.3.1 Registration Messages
ProcessorRegistrationMessage
csharp
public class ProcessorRegistrationMessage
{
    public string ProcessorId { get; set; }
    public string ProcessorType { get; set; }
    public string Version { get; set; }
    public SchemaDefinition InputSchema { get; set; }
    public SchemaDefinition OutputSchema { get; set; }
    public List<string> Capabilities { get; set; }
    public Dictionary<string, ParameterDefinition> ConfigurationParameters { get; set; }
}
ImporterRegistrationMessage
csharp
public class ImporterRegistrationMessage
{
    public string ImporterId { get; set; }
    public string Protocol { get; set; }
    public string Version { get; set; }
    public List<string> Capabilities { get; set; }
    public SchemaDefinition OutputSchema { get; set; }
    public Dictionary<string, ParameterDefinition> ConfigurationParameters { get; set; }
    public ProtocolCapabilities ProtocolCapabilities { get; set; }
}
ExporterRegistrationMessage
csharp
public class ExporterRegistrationMessage
{
    public string ExporterId { get; set; }
    public string Protocol { get; set; }
    public string Version { get; set; }
    public List<string> Capabilities { get; set; }
    public SchemaDefinition InputSchema { get; set; }
    public Dictionary<string, ParameterDefinition> ConfigurationParameters { get; set; }
    public ProtocolCapabilities ProtocolCapabilities { get; set; }
    public List<string> SupportedMergeStrategies { get; set; }
}
Registration Acknowledgment Messages
csharp
public class RegistrationAcknowledgmentMessage
{
    public string ComponentId { get; set; }
    public string ComponentType { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public List<ValidationError> Errors { get; set; }
}
6.3.2 Command Messages
ProcessCommandMessage
csharp
public class ProcessCommandMessage
{
    public string ProcessorId { get; set; }
    public string ExecutionId { get; set; }
    public string FlowId { get; set; }
    public string BranchPath { get; set; }
    public string StepId { get; set; }
    public string InputDataLocation { get; set; }
    public string OutputDataLocation { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}
ImportCommandMessage
csharp
public class ImportCommandMessage
{
    public string ImporterId { get; set; }
    public string ExecutionId { get; set; }
    public string FlowId { get; set; }
    public string OutputDataLocation { get; set; }
    public Dictionary<string, object> ConnectionParameters { get; set; }
    public Dictionary<string, object> ImportParameters { get; set; }
}
ExportCommandMessage
csharp
public class ExportCommandMessage
{
    public string ExporterId { get; set; }
    public string ExecutionId { get; set; }
    public string FlowId { get; set; }
    public string InputDataLocation { get; set; }
    public Dictionary<string, object> ConnectionParameters { get; set; }
    public Dictionary<string, object> ExportParameters { get; set; }
    public MergeStrategy MergeStrategy { get; set; }
    public List<string> MergeBranchPaths { get; set; }
}
6.3.3 Status and Completion Messages
StatusUpdateMessage
csharp
public class StatusUpdateMessage
{
    public string ComponentId { get; set; }
    public string ComponentType { get; set; }
    public string Status { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Metrics { get; set; }
}
ProcessCompletionMessage
csharp
public class ProcessCompletionMessage
{
    public string ProcessorId { get; set; }
    public string ExecutionId { get; set; }
    public string FlowId { get; set; }
    public string BranchPath { get; set; }
    public string StepId { get; set; }
    public bool Success { get; set; }
    public ExecutionStatistics Statistics { get; set; }
    public List<ErrorDetail> Errors { get; set; }
}
ImportCompletionMessage
csharp
public class ImportCompletionMessage
{
    public string ImporterId { get; set; }
    public string ExecutionId { get; set; }
    public string FlowId { get; set; }
    public bool Success { get; set; }
    public ExecutionStatistics Statistics { get; set; }
    public int RecordCount { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
    public List<ErrorDetail> Errors { get; set; }
}
ExportCompletionMessage
csharp
public class ExportCompletionMessage
{
    public string ExporterId { get; set; }
    public string ExecutionId { get; set; }
    public string FlowId { get; set; }
    public bool Success { get; set; }
    public ExecutionStatistics Statistics { get; set; }
    public int RecordCount { get; set; }
    public Dictionary<string, object> DeliveryReceipt { get; set; }
    public List<ErrorDetail> Errors { get; set; }
}
6.4 Component Implementation Patterns
The FlowOrchestrator system defines standard patterns for component implementation to ensure consistency and reliability.

6.4.1 Console Application Pattern
Components follow a standard console application pattern:

csharp
// Program.cs
class Program
{
    static async Task Main(string[] args)
    {
        // 1. Load configuration
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
            
        // 2. Configure services
        var services = new ServiceCollection();
        
        // Add infrastructure
        ConfigureMassTransit(services, config);
        ConfigureOpenTelemetry(services, config);
        
        // Add component-specific services
        services.AddSingleton<IProcessorService, ConcreteProcessorImplementation>();
        services.AddSingleton<ISchemaProvider, ConcreteSchemaProvider>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 3. Initialize component
        var processor = serviceProvider.GetRequiredService<IProcessorService>();
        
        // 4. Perform self-registration via message bus
        await SendRegistrationMessage(serviceProvider);
        
        // 5. Run until terminated
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) => { cts.Cancel(); e.Cancel = true; };
        
        await processor.RunAsync(cts.Token);
    }
    
    private static void ConfigureMassTransit(IServiceCollection services, IConfiguration config)
    {
        services.AddMassTransit(busConfig =>
        {
            // Register message consumers
            busConfig.AddConsumer<CommandConsumer>();
            
            busConfig.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(config["RabbitMQ:Host"], config["RabbitMQ:VirtualHost"], h =>
                {
                    h.Username(config["RabbitMQ:Username"]);
                    h.Password(config["RabbitMQ:Password"]);
                });
                
                // Configure endpoint for receiving commands
                cfg.ReceiveEndpoint($"{ComponentType}-commands", e =>
                {
                    e.Consumer<CommandConsumer>(context);
                });
            });
        });
    }
}
6.4.2 Entity Manager WebAPI Pattern
Entity Managers follow a standard WebAPI pattern:

csharp
// Startup.cs
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Configure controllers
        services.AddControllers();
        
        // Configure MongoDB
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = MongoClientSettings.FromConnectionString(
                Configuration["MongoDB:ConnectionString"]);
            return new MongoClient(settings);
        });
        
        services.AddSingleton<IEntityRepository, EntityRepository>();
        
        // Configure MassTransit
        services.AddMassTransit(busConfig =>
        {
            // Register message consumers
            busConfig.AddConsumer<RegistrationConsumer>();
            busConfig.AddConsumer<StatusConsumer>();
            
            busConfig.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(Configuration["RabbitMQ:Host"], Configuration["RabbitMQ:VirtualHost"], h =>
                {
                    h.Username(Configuration["RabbitMQ:Username"]);
                    h.Password(Configuration["RabbitMQ:Password"]);
                });
                
                // Configure endpoints for message consumers
                cfg.ReceiveEndpoint($"{EntityType}-registration", e =>
                {
                    e.ConfigureConsumer<RegistrationConsumer>(context);
                });
                
                cfg.ReceiveEndpoint($"{EntityType}-status", e =>
                {
                    e.ConfigureConsumer<StatusConsumer>(context);
                });
            });
        });
        
        // Configure OpenTelemetry
        services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .AddSource("EntityManager")
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("EntityManager"))
                .AddAspNetCoreInstrumentation()
                .AddMongoDBInstrumentation()
                .AddOtlpExporter(options => 
                {
                    options.Endpoint = new Uri(Configuration["OpenTelemetry:Endpoint"]);
                }));
        
        // Add Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Entity Manager API", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Entity Manager API v1"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
6.4.3 Message Consumer Pattern
Components implement message consumers to handle commands:

csharp
// CommandConsumer.cs
public class CommandConsumer : IConsumer<CommandMessage>
{
    private readonly IComponentService _service;
    private readonly ILogger<CommandConsumer> _logger;

    public CommandConsumer(
        IComponentService service,
        ILogger<CommandConsumer> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CommandMessage> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("Received command for component {ComponentId}", message.ComponentId);
        
        try
        {
            // Update component state
            _service.SetState(ComponentState.PROCESSING);
            
            // Process the command
            var result = await _service.ProcessAsync(message);
            
            // Send completion message
            await context.Publish(new CompletionMessage
            {
                ComponentId = message.ComponentId,
                ExecutionId = message.ExecutionId,
                Success = true,
                Results = result
            });
            
            // Update component state
            _service.SetState(ComponentState.READY);
            
            _logger.LogInformation("Command processed successfully for {ComponentId}", message.ComponentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process command for {ComponentId}", message.ComponentId);
            
            // Update component state
            _service.SetState(ComponentState.ERROR);
            
            // Send failure message
            await context.Publish(new CompletionMessage
            {
                ComponentId = message.ComponentId,
                ExecutionId = message.ExecutionId,
                Success = false,
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        ErrorType = "PROCESSING_ERROR",
                        Message = ex.Message,
                        Details = ex.StackTrace
                    }
                }
            });
            
            // Try to recover
            await _service.TryRecoverAsync();
        }
    }
}
6.4.4 Repository Pattern
Entity Managers implement the repository pattern for MongoDB data access:

csharp
// EntityRepository.cs
public class EntityRepository<TEntity> : IEntityRepository<TEntity>
    where TEntity : class
{
    private readonly IMongoCollection<TEntity> _collection;

    public EntityRepository(IMongoClient client, IConfiguration configuration, string collectionName)
    {
        var database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
        _collection = database.GetCollection<TEntity>(collectionName);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _collection.Find(Builders<TEntity>.Filter.Empty).ToListAsync();
    }

    public async Task<TEntity> GetByIdAsync(string id)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TEntity>> GetByFilterAsync(FilterDefinition<TEntity> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task CreateAsync(TEntity entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(string id, TEntity entity)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", id);
        await _collection.DeleteOneAsync(filter);
    }
}
6.4.5 Controller Pattern
Entity Managers implement CRUD controllers for REST API:

csharp
// EntityController.cs
[ApiController]
[Route("api/[controller]")]
public class EntityController<TEntity> : ControllerBase
    where TEntity : class
{
    private readonly IEntityRepository<TEntity> _repository;
    private readonly ILogger<EntityController<TEntity>> _logger;

    public EntityController(
        IEntityRepository<TEntity> repository,
        ILogger<EntityController<TEntity>> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TEntity>>> GetAll()
    {
        var entities = await _repository.GetAllAsync();
        return Ok(entities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TEntity>> GetById(string id)
    {
        var entity = await _repository.GetByIdAsync(id);
        
        if (entity == null)
        {
            return NotFound();
        }
        
        return Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<TEntity>> Create(TEntity entity)
    {
        await _repository.CreateAsync(entity);
        
        // Use reflection to get the ID field
        var idProperty = typeof(TEntity).GetProperty("Id") ?? typeof(TEntity).GetProperty("EntityId");
        var id = idProperty?.GetValue(entity).ToString();
        
        return CreatedAtAction(nameof(GetById), new { id = id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, TEntity entity)
    {
        var existingEntity = await _repository.GetByIdAsync(id);
        
        if (existingEntity == null)
        {
            return NotFound();
        }
        
        await _repository.UpdateAsync(id, entity);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingEntity = await _repository.GetByIdAsync(id);
        
        if (existingEntity == null)
        {
            return NotFound();
        }
        
        await _repository.DeleteAsync(id);
        
        return NoContent();
    }
}
6.5 Entity Data Models
The Entity Managers use the following data models for storing component metadata in MongoDB.

6.5.1 Processor Entity
csharp
public class ProcessorEntity
{
    [BsonId]
    public string ProcessorId { get; set; }
    
    public string ProcessorType { get; set; }
    
    public string Version { get; set; }
    
    public SchemaDefinition InputSchema { get; set; }
    
    public SchemaDefinition OutputSchema { get; set; }
    
    public List<string> Capabilities { get; set; }
    
    public Dictionary<string, ParameterDefinition> ConfigurationParameters { get; set; }
    
    public string Status { get; set; }
    
    public DateTime RegistrationTimestamp { get; set; }
    
    public DateTime LastUpdateTimestamp { get; set; }
    
    public string LastKnownAddress { get; set; }
}
6.5.2 Importer Entity
csharp
public class ImporterEntity
{
    [BsonId]
    public string ImporterId { get; set; }
    
    public string Protocol { get; set; }
    
    public string Version { get; set; }
    
    public List<string> Capabilities { get; set; }
    
    public SchemaDefinition OutputSchema { get; set; }
    
    public Dictionary<string, ParameterDefinition> ConfigurationParameters { get; set; }
    
    public ProtocolCapabilities ProtocolCapabilities { get; set; }
    
    public string Status { get; set; }
    
    public DateTime RegistrationTimestamp { get; set; }
    
    public DateTime LastUpdateTimestamp { get; set; }
    
    public string LastKnownAddress { get; set; }
}
6.5.3 Exporter Entity
csharp
public class ExporterEntity
{
    [BsonId]
    public string ExporterId { get; set; }
    
    public string Protocol { get; set; }
    
    public string Version { get; set; }
    
    public List<string> Capabilities { get; set; }
    
    public SchemaDefinition InputSchema { get; set; }
    
    public Dictionary<string, ParameterDefinition> ConfigurationParameters { get; set; }
    
    public ProtocolCapabilities ProtocolCapabilities { get; set; }
    
    public List<string> SupportedMergeStrategies { get; set; }
    
    public string Status { get; set; }
    
    public DateTime RegistrationTimestamp { get; set; }
    
    public DateTime LastUpdateTimestamp { get; set; }
    
    public string LastKnownAddress { get; set; }
}
6.5.4 Flow Entity
csharp
public class FlowEntity
{
    [BsonId]
    public string FlowId { get; set; }
    
    public string Version { get; set; }
    
    public string Description { get; set; }
    
    public string ImporterServiceId { get; set; }
    
    public string ImporterServiceVersion { get; set; }
    
    public List<ProcessingChainReference> ProcessingChains { get; set; }
    
    public List<ExporterReference> Exporters { get; set; }
    
    public Dictionary<string, ConnectionDefinition> Connections { get; set; }
    
    public string Status { get; set; }
    
    public DateTime CreatedTimestamp { get; set; }
    
    public DateTime LastModifiedTimestamp { get; set; }
    
    public string CreatedBy { get; set; }
    
    public string LastModifiedBy { get; set; }
}
6.6 Management Considerations
The following considerations should be taken into account when implementing the Entity Managers and components:

6.6.1 Versioning and Compatibility
Component versions follow semantic versioning (MAJOR.MINOR.PATCH)
Breaking changes require MAJOR version increment
Feature additions require MINOR version increment
Bug fixes require PATCH version increment
Components include version compatibility information in registration
Entity Managers validate version compatibility during registration
Multiple versions of the same component can be registered simultaneously
The Orchestrator chooses appropriate component version during flow execution
Version status transitions (ACTIVE -> DEPRECATED -> ARCHIVED) are managed by Entity Managers
6.6.2 Error Handling and Recovery
Components implement comprehensive error handling
Error details are included in completion messages
Components attempt self-recovery from error states
Entity Managers track component error states
The Orchestrator implements retry and fallback strategies
Error logs include correlation IDs for tracking
Components publish status updates during recovery
Entity Managers maintain error history for components
6.6.3 Monitoring and Observability
Components include OpenTelemetry integration
Metrics, traces, and logs follow standardized formats
Entity Managers track component health and status
The Statistics Service aggregates performance metrics
Dashboard visualization for component status
Alert triggers for component errors and health issues
Performance optimization based on collected metrics
Historical trend analysis for component performance
6.6.4 Deployment and Scalability
Components are containerized for deployment
Each component can be scaled independently
Load balancing for Entity Managers
Auto-scaling based on message queue depth
Deployment automation through CI/CD pipelines
Configuration management through environment variables
Health checks for deployment validation
Canary deployments for new component versions
