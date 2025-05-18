# Component Class Structures

This document provides detailed class structures for the core components of the FlowOrchestrator system: Importers, Processors, and Exporters. It includes implementation patterns, inheritance hierarchies, and integration details.

## 1. Importer Class Structure

### 1.1 Abstract Base Classes

```
FlowOrchestrator.ImporterBase/
├── Program.cs                             # Entry point with infrastructure setup
├── AbstractImporterApplication.cs         # Abstract base class for importer applications
│   ├── Main()                             # Entry point that configures and runs the importer
│   ├── ConfigureServices()                # Configures dependency injection
│   ├── ConfigureMassTransit()             # Configures message bus
│   ├── ConfigureOpenTelemetry()           # Configures observability
│   └── RegisterWithEntityManager()         # Handles self-registration
│
├── AbstractImporterService.cs             # Abstract base class for importer services
│   ├── Protocol { get; }                  # Abstract property for protocol identification
│   ├── ImporterId { get; }                # Abstract property for importer identification
│   ├── ImporterName { get; }              # Abstract property for importer name
│   ├── ImporterType { get; }              # Abstract property for importer type
│   ├── Version { get; }                   # Abstract property for version information
│   ├── GetCapabilities()                  # Abstract method for capability reporting
│   ├── Import()                           # Abstract method for import operation
│   ├── GetOutputSchema()                  # Abstract method for schema definition
│   ├── Consume()                          # Message consumer implementation
│   └── ValidateConfiguration()            # Configuration validation
│
├── Infrastructure/
│   ├── MassTransit/
│   │   ├── CommandConsumer.cs             # Consumes import commands
│   │   ├── RegistrationPublisher.cs       # Publishes registration messages
│   │   └── BusConfiguration.cs            # Message bus configuration
│   ├── OpenTelemetry/
│   │   └── TelemetryConfiguration.cs      # Observability configuration
│   └── Configuration/
│       └── ConfigurationLoader.cs         # Configuration loading
│
└── Models/
    ├── ImportParameters.cs                # Parameters for import operation
    ├── ImportResult.cs                    # Result of import operation
    ├── ProtocolCapabilities.cs            # Protocol capability model
    └── SchemaDefinition.cs                # Schema definition model
```

### 1.2 Concrete Implementation

```
FlowOrchestrator.FileImporter/
├── FileImporterApplication.cs             # Concrete importer application
│   └── ConfigureServices()                # Configures file-specific services
│
├── FileImporterService.cs                 # Concrete importer service
│   ├── Protocol { get; }                  # Returns "FILE"
│   ├── ImporterId { get; }                # Returns unique ID
│   ├── ImporterName { get; }              # Returns "File Importer"
│   ├── ImporterType { get; }              # Returns importer type
│   ├── Version { get; }                   # Returns version information
│   ├── GetCapabilities()                  # Returns file-specific capabilities
│   ├── Import()                           # Implements file import logic
│   ├── GetOutputSchema()                  # Returns file data schema
│   └── ValidateConfiguration()            # Validates file-specific configuration
│
├── FileHandling/
│   ├── FileReader.cs                      # Handles file reading operations
│   ├── FileWatcher.cs                     # Monitors for file changes
│   └── FileParser.cs                      # Parses file content
│
└── Configuration/
    ├── FileImporterOptions.cs             # File-specific configuration options
    └── appsettings.json                   # Configuration file
```

## 2. Processor Class Structure

### 2.1 Abstract Base Classes

```
FlowOrchestrator.ProcessorBase/
├── Program.cs                             # Entry point with infrastructure setup
├── AbstractProcessorApplication.cs        # Abstract base class for processor applications
│   ├── Main()                             # Entry point that configures and runs the processor
│   ├── ConfigureServices()                # Configures dependency injection
│   ├── ConfigureMassTransit()             # Configures message bus
│   ├── ConfigureOpenTelemetry()           # Configures observability
│   └── RegisterWithEntityManager()         # Handles self-registration
│
├── AbstractProcessorService.cs            # Abstract base class for processor services
│   ├── ProcessorId { get; }               # Abstract property for processor identification
│   ├── ProcessorName { get; }             # Abstract property for processor name
│   ├── ProcessorType { get; }             # Abstract property for processor type
│   ├── Version { get; }                   # Abstract property for version information
│   ├── Process()                          # Abstract method for processing operation
│   ├── GetInputSchema()                   # Abstract method for input schema definition
│   ├── GetOutputSchema()                  # Abstract method for output schema definition
│   ├── Consume()                          # Message consumer implementation
│   └── ValidateConfiguration()            # Configuration validation
│
├── Infrastructure/
│   ├── MassTransit/
│   │   ├── CommandConsumer.cs             # Consumes process commands
│   │   ├── RegistrationPublisher.cs       # Publishes registration messages
│   │   └── BusConfiguration.cs            # Message bus configuration
│   ├── OpenTelemetry/
│   │   └── TelemetryConfiguration.cs      # Observability configuration
│   └── Configuration/
│       └── ConfigurationLoader.cs         # Configuration loading
│
└── Models/
    ├── ProcessParameters.cs               # Parameters for process operation
    ├── ProcessingResult.cs                # Result of processing operation
    ├── SchemaDefinition.cs                # Schema definition model
    └── ValidationResult.cs                # Validation result model
### 2.2 Concrete Implementation

```
FlowOrchestrator.JsonProcessor/
├── JsonProcessorApplication.cs            # Concrete processor application
│   └── ConfigureServices()                # Configures JSON-specific services
│
├── JsonProcessorService.cs                # Concrete processor service
│   ├── ProcessorId { get; }               # Returns unique ID
│   ├── ProcessorName { get; }             # Returns "JSON Processor"
│   ├── ProcessorType { get; }             # Returns "TRANSFORMATION"
│   ├── Version { get; }                   # Returns version information
│   ├── Process()                          # Implements JSON processing logic
│   ├── GetInputSchema()                   # Returns JSON input schema
│   ├── GetOutputSchema()                  # Returns JSON output schema
│   └── ValidateConfiguration()            # Validates JSON-specific configuration
│
├── Transformation/
│   ├── JsonTransformationEngine.cs        # Core JSON transformation logic
│   ├── JsonPathEvaluator.cs               # Evaluates JSON paths
│   └── JsonSchemaValidator.cs             # Validates JSON against schemas
│
└── Configuration/
    ├── JsonProcessorOptions.cs            # JSON-specific configuration options
    └── appsettings.json                   # Configuration file
```

## 3. Exporter Class Structure

### 3.1 Abstract Base Classes

FlowOrchestrator.ExporterBase/
├── Program.cs                             # Entry point with infrastructure setup
├── AbstractExporterApplication.cs         # Abstract base class for exporter applications
│   ├── Main()                             # Entry point that configures and runs the exporter
│   ├── ConfigureServices()                # Configures dependency injection
│   ├── ConfigureMassTransit()             # Configures message bus
│   ├── ConfigureOpenTelemetry()           # Configures observability
│   └── RegisterWithEntityManager()         # Handles self-registration
│
├── AbstractExporterService.cs             # Abstract base class for exporter services
│   ├── Protocol { get; }                  # Abstract property for protocol identification
│   ├── ExporterId { get; }                # Abstract property for exporter identification
│   ├── ExporterName { get; }              # Abstract property for exporter name
│   ├── ExporterType { get; }              # Abstract property for exporter type
│   ├── Version { get; }                   # Abstract property for version information
│   ├── GetCapabilities()                  # Abstract method for capability reporting
│   ├── GetMergeCapabilities()             # Abstract method for merge capability reporting
│   ├── Export()                           # Abstract method for export operation
│   ├── MergeBranches()                    # Abstract method for branch merging
│   ├── Consume()                          # Message consumer implementation
│   └── ValidateConfiguration()            # Configuration validation
│
├── Infrastructure/
│   ├── MassTransit/
│   │   ├── CommandConsumer.cs             # Consumes export commands
│   │   ├── RegistrationPublisher.cs       # Publishes registration messages
│   │   └── BusConfiguration.cs            # Message bus configuration
│   ├── OpenTelemetry/
│   │   └── TelemetryConfiguration.cs      # Observability configuration
│   └── Configuration/
│       └── ConfigurationLoader.cs         # Configuration loading
│
└── Models/
    ├── ExportParameters.cs                # Parameters for export operation
    ├── ExportResult.cs                    # Result of export operation
    ├── ProtocolCapabilities.cs            # Protocol capability model
    ├── MergeCapabilities.cs               # Merge capability model
    ├── MergeStrategy.cs                   # Branch merge strategy model
    └── SchemaDefinition.cs                # Schema definition model
```

### 3.2 Concrete Implementation

FlowOrchestrator.FileExporter/
├── FileExporterApplication.cs             # Concrete exporter application
│   └── ConfigureServices()                # Configures file-specific services
│
├── FileExporterService.cs                 # Concrete exporter service
│   ├── Protocol { get; }                  # Returns "FILE"
│   ├── ExporterId { get; }                # Returns unique ID
│   ├── ExporterName { get; }              # Returns "File Exporter"
│   ├── ExporterType { get; }              # Returns exporter type
│   ├── Version { get; }                   # Returns version information
│   ├── GetCapabilities()                  # Returns file-specific capabilities
│   ├── GetMergeCapabilities()             # Returns merge capabilities
│   ├── Export()                           # Implements file export logic
│   ├── MergeBranches()                    # Implements branch merging for files
│   └── ValidateConfiguration()            # Validates file-specific configuration
│
├── FileHandling/
│   ├── FileWriter.cs                      # Handles file writing operations
│   ├── FileMerger.cs                      # Merges multiple files
│   └── FileFormatter.cs                   # Formats data for file output
│
└── Configuration/
    ├── FileExporterOptions.cs             # File-specific configuration options
    └── appsettings.json                   # Configuration file
```

## 4. Common Implementation Patterns

### 4.1 Self-Registration Pattern

All components (Importers, Processors, and Exporters) implement a self-registration pattern:

```csharp
// Example from AbstractImporterApplication
protected virtual void RegisterWithEntityManager()
{
    var registrationMessage = new ImporterRegistrationMessage
    {
        ImporterId = _importerService.ImporterId,
        ImporterName = _importerService.ImporterName,
        ImporterType = _importerService.ImporterType,
        Version = _importerService.Version,
        Protocol = _importerService.Protocol,
        Capabilities = _importerService.GetCapabilities(),
        OutputSchema = _importerService.GetOutputSchema()
    };

    _registrationPublisher.PublishRegistration(registrationMessage);
    _logger.LogInformation("Importer registration message published");
}