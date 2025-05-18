# FlowOrchestrator Architecture Overview

This document provides a comprehensive overview of the FlowOrchestrator system architecture, focusing on the relationships between abstract classes, common components, concrete classes, and other implemented objects.

## 1. System Architecture

The FlowOrchestrator system follows a modular service-oriented architecture with clear separation of concerns across specialized components. The system is organized into several domains:

1. **Core Domain**: Common utilities, abstractions, and domain models
2. **Execution Domain**: Orchestration, memory management, and branch control
3. **Integration Domain**: Importers and exporters for data ingestion and delivery
4. **Processing Domain**: Processors for data transformation
5. **Management Domain**: Entity managers for metadata management and validation
6. **Observability Domain**: Monitoring, statistics, and analytics

## 2. Project Types

All components in the system are implemented using .NET 9.0 project types:

1. **Core Libraries**: Class Library (.NET 9)
   - Fundamental abstractions, interfaces, and domain models
   - Shared utilities and helpers
   - Common infrastructure components

2. **Domain Base Projects**: Class Library (.NET 9)
   - Shared infrastructure for each domain
   - Abstract base classes with common functionality
   - Cross-cutting concerns implementation

3. **Service Base Projects**: Console Application (.NET 9) / ASP.NET Core Web API (.NET 9)
   - Abstract base applications for each service type
   - Service-specific infrastructure
   - Message handling and registration

4. **Concrete Implementations**: Console Application (.NET 9) / ASP.NET Core Web API (.NET 9)
   - Specific implementations of services
   - Protocol-specific or transformation-specific logic
   - Configuration and customization

5. **Worker Services**: Worker Service (.NET 9)
   - Background processing services
   - Orchestration and coordination
   - Scheduling and monitoring

6. **Test Projects**: xUnit Test Project (.NET 9)
   - Unit tests
   - Integration tests
   - System tests

## 3. Abstract Base Pattern

The system implements a consistent abstract base pattern across all domains:

### 3.1 Core Domain

```
FlowOrchestrator.Abstractions/
├── IServiceBase                           # Base interface for all services
│   ├── IImporterService                   # Interface for importers
│   ├── IProcessorService                  # Interface for processors
│   └── IExporterService                   # Interface for exporters
│
├── IEntity                                # Base interface for all entities
│   ├── IFlowEntity                        # Interface for flow entities
│   ├── IProcessingChainEntity             # Interface for processing chain entities
│   ├── ISourceEntity                      # Interface for source entities
│   ├── IDestinationEntity                 # Interface for destination entities
│   ├── ISourceAssignmentEntity            # Interface for source assignment entities
│   ├── IDestinationAssignmentEntity       # Interface for destination assignment entities
│   └── IScheduledFlowEntity               # Interface for scheduled flow entities
```

### 3.2 Integration Domain

```
FlowOrchestrator.IntegrationBase/
├── AbstractIntegrationComponent           # Shared base class for integration components
│
├── FlowOrchestrator.ImporterBase/
│   ├── AbstractImporterApplication        # Abstract base class for importer applications
│   └── AbstractImporterService            # Abstract base class for importer services
│       └── FileImporterService            # Concrete importer service
│
└── FlowOrchestrator.ExporterBase/
    ├── AbstractExporterApplication        # Abstract base class for exporter applications
    └── AbstractExporterService            # Abstract base class for exporter services
        └── FileExporterService            # Concrete exporter service
```

### 3.3 Processing Domain

```
FlowOrchestrator.ProcessingBase/
├── AbstractProcessingComponent            # Shared base class for processing components
│
└── FlowOrchestrator.ProcessorBase/
    ├── AbstractProcessorApplication       # Abstract base class for processor applications
    └── AbstractProcessorService           # Abstract base class for processor services
        └── JsonProcessorService           # Concrete processor service
```

### 3.4 Management Domain

```
FlowOrchestrator.EntityManagerBase/
├── AbstractEntityManagerApplication       # Abstract base class for entity manager applications
├── AbstractEntityController               # Abstract base class for entity controllers
└── AbstractManagerService<TService, TServiceId>  # Abstract base class for manager services
    ├── ImporterServiceManager             # Concrete manager service
    ├── ProcessorServiceManager            # Concrete manager service
    ├── ExporterServiceManager             # Concrete manager service
    ├── FlowEntityManager                  # Concrete manager service
    ├── SourceEntityManager                # Concrete manager service
    ├── DestinationEntityManager           # Concrete manager service
    ├── SourceAssignmentEntityManager      # Concrete manager service
    ├── DestinationAssignmentEntityManager # Concrete manager service
    └── ScheduledFlowEntityManager         # Concrete manager service

### 3.5 Execution Domain

```
FlowOrchestrator.ExecutionBase/
├── AbstractExecutionComponent             # Shared base class for execution components
│
├── FlowOrchestrator.Orchestrator/         # Flow execution coordination
├── FlowOrchestrator.MemoryManager/        # Memory management
├── FlowOrchestrator.BranchManager/        # Branch execution coordination
└── FlowOrchestrator.TaskScheduler/        # Task scheduling
```

### 3.6 Observability Domain

```
FlowOrchestrator.ObservabilityBase/
├── AbstractObservabilityComponent         # Shared base class for observability components
│
├── FlowOrchestrator.StatisticsService/    # Statistics collection and analysis
├── FlowOrchestrator.MonitoringFramework/  # Monitoring and visualization
├── FlowOrchestrator.AlertingSystem/       # Alerting and notification
└── FlowOrchestrator.AnalyticsEngine/      # Analytics and reporting
```

## 4. Component Relationships

The components in the system interact through well-defined relationships:

### 4.1 Inheritance Relationships

```
AbstractServiceBase
├── AbstractImporterService
│   └── FileImporterService
│
├── AbstractProcessorService
│   └── JsonProcessorService
│
└── AbstractExporterService
    └── FileExporterService
```

```
AbstractEntity
├── AbstractFlowEntity
├── AbstractProcessingChainEntity
├── AbstractSourceEntity
├── AbstractDestinationEntity
├── AbstractSourceAssignmentEntity
├── AbstractDestinationAssignmentEntity
├── AbstractScheduledFlowEntity
└── AbstractTaskSchedulerEntity
```

### 4.2 Communication Relationships

Components communicate through message-based interactions:

1. **Registration Flow**: Components register with Entity Managers
2. **Command Flow**: Orchestrator sends commands to components
3. **Result Flow**: Components send results back to Orchestrator
4. **Validation Flow**: Entity Managers validate component compatibility
5. **Observability Flow**: Components report telemetry data

### 4.3 Validation Relationships

Entity Managers validate the compatibility of components:

1. **ProcessingChainEntityManager**: Validates processor compatibility
2. **FlowEntityManager**: Validates importer/exporter/chain compatibility
3. **SourceAssignmentEntityManager**: Validates source-importer compatibility
4. **DestinationAssignmentEntityManager**: Validates destination-exporter compatibility
5. **ScheduledFlowEntityManager**: Validates scheduled flow compatibility

## 5. Implementation Patterns

The system implements several common patterns across all components:

### 5.1 Self-Registration Pattern

All components self-register with their respective Entity Managers:

```csharp
protected virtual void RegisterWithEntityManager()
{
    var registrationMessage = new RegistrationMessage
    {
        ComponentId = ComponentId,
        ComponentName = ComponentName,
        ComponentType = ComponentType,
        Version = Version,
        // Component-specific properties
    };

    _registrationPublisher.PublishRegistration(registrationMessage);
}
```

### 5.2 Message-Based Communication Pattern

All components communicate through the message bus:

```csharp
public virtual async Task Consume(ConsumeContext<Command> context)
{
    try
    {
        // Process command
        var result = ProcessCommand(context.Message);

        // Publish result
        await context.Publish(new CommandResult
        {
            CommandId = context.Message.CommandId,
            // Result properties
        });
    }
    catch (Exception ex)
    {
        // Publish error
        await PublishError(context, ex);
    }
}
```

### 5.3 Infrastructure Integration Pattern

All components integrate with common infrastructure:

1. **MassTransit**: For message-based communication
2. **OpenTelemetry**: For observability
3. **MongoDB**: For persistent storage (Entity Managers)

## 6. Conclusion

The FlowOrchestrator system implements a consistent architectural pattern across all domains, with clear separation of concerns and well-defined relationships between components. The abstract base pattern ensures consistency and reduces code duplication, while the message-based communication pattern enables loose coupling and scalability.
