# Abstract Processor Implementation Pattern

## Overview

The Abstract Processor pattern in the FlowOrchestrator system provides a standardized approach for implementing processor components. This pattern ensures that all processors share common infrastructure while allowing concrete implementations to focus solely on their specific processing logic.

## Key Characteristics

1. **Complete Infrastructure in Base Class**: The abstract processor application includes all required infrastructure:
   - MassTransit integration for message bus communication
   - OpenTelemetry integration for observability
   - Lifecycle management
   - Self-registration with entity managers
   - Error handling and recovery
   - Configuration management

2. **Minimal Implementation in Concrete Classes**: Concrete processor implementations only need to provide:
   - Processor identity properties (ID, name, type, version)
   - Core processing logic (Process method)
   - Schema definitions (input and output)
   - Capabilities list

## Implementation Pattern

### Abstract Processor Base Class

```csharp
public abstract class AbstractProcessorApplication
{
    // Main entry point - this is in the base class
    public static void Main(string[] args)
    {
        // Create the concrete processor instance
        var processor = CreateProcessor();
        
        // Configure services
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();
        
        // Initialize and run
        processor.Initialize();
        processor.RegisterWithEntityManager();
        
        // Wait for termination signal
        Console.CancelKeyPress += (s, e) => { processor.Terminate(); };
        
        // Keep the application running
        Thread.Sleep(Timeout.Infinite);
    }
    
    // All infrastructure setup and lifecycle management methods
    // ...
    
    // Message handling
    public void HandleProcessCommand(ProcessCommand command) 
    {
        // State management, error handling, etc.
        // ...
        
        // Call the virtual method implemented by concrete classes
        var result = Process(command.Parameters, command.Context);
        
        // Result publishing, state management, etc.
        // ...
    }
    
    // Abstract properties that define processor identity
    public abstract string ProcessorId { get; }
    public abstract string ProcessorName { get; }
    public abstract string ProcessorType { get; }
    public abstract string Version { get; }
    
    // Abstract methods that define processor behavior
    protected abstract ProcessingResult Process(ProcessParameters parameters, ExecutionContext context);
    protected abstract SchemaDefinition GetInputSchema();
    protected abstract SchemaDefinition GetOutputSchema();
    protected virtual List<string> GetCapabilities() => new List<string>();
}
```

### Concrete Processor Implementation

```csharp
public class JsonProcessor : AbstractProcessorApplication
{
    // Identity properties
    public override string ProcessorId => "JSON-PROC-001";
    public override string ProcessorName => "JSON Transformation Processor";
    public override string ProcessorType => "TRANSFORMATION";
    public override string Version => "1.0.0";
    
    // Core processing logic
    protected override ProcessingResult Process(ProcessParameters parameters, ExecutionContext context)
    {
        // Implement JSON processing logic
        var result = new ProcessingResult();
        // Transform data...
        return result;
    }
    
    // Schema definitions
    protected override SchemaDefinition GetInputSchema()
    {
        return new SchemaDefinition
        {
            // Define input schema...
        };
    }
    
    protected override SchemaDefinition GetOutputSchema()
    {
        return new SchemaDefinition
        {
            // Define output schema...
        };
    }
    
    // Optional capabilities
    protected override List<string> GetCapabilities()
    {
        return new List<string>
        {
            "JSON_PATH_QUERY",
            "JSON_TRANSFORMATION",
            "SCHEMA_VALIDATION"
        };
    }
}
```

## Registration Process

The abstract processor handles self-registration with the entity manager:

```csharp
private void RegisterWithEntityManager()
{
    var registrationMessage = new ProcessorRegistrationMessage
    {
        ProcessorId = ProcessorId,
        ProcessorName = ProcessorName,
        ProcessorType = ProcessorType,
        Version = Version,
        InputSchema = GetInputSchema(),
        OutputSchema = GetOutputSchema(),
        Capabilities = GetCapabilities(),
        // Other properties...
    };
    
    _messageBus.Publish(registrationMessage);
}
```

This registration message is consumed by the Processor Entity Manager, which creates a ProcessorEntity based on the provided metadata.

## Best Practices

1. **Focus on Core Logic**: Concrete implementations should focus solely on their specific processing logic and metadata.

2. **Avoid Infrastructure Code**: Concrete implementations should never include infrastructure setup or message handling code.

3. **Clear Documentation**: Document the purpose and capabilities of each concrete processor.

4. **Version Management**: Follow semantic versioning principles for the Version property.

5. **Schema Design**: Design schemas carefully to ensure compatibility with upstream and downstream components.

6. **Capabilities Declaration**: Declare capabilities accurately to enable proper validation by entity managers.

7. **Testing**: Create unit tests that focus on the Process method and schema definitions.

## Benefits

1. **Consistency**: All processors follow the same patterns for infrastructure setup and communication.

2. **Reduced Duplication**: Infrastructure code is defined once in the base class.

3. **Simplified Development**: Developers can focus on implementing the specific processing logic.

4. **Standardized Registration**: All processors register with entity managers in a consistent way.

5. **Uniform Error Handling**: Error handling and recovery follow a standardized approach.

This pattern applies similarly to Importers and Exporters, with appropriate adjustments for their specific requirements.
