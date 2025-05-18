# Comprehensive Textual UML View of FlowOrchestrator System

This document provides a comprehensive textual UML view of the relationships between abstract classes, common components, concrete classes, and other implemented objects in the FlowOrchestrator system. All components are shown with their .NET 9.0 project types.

```
+----------------------------------------------------------------------------------------------------------+
|                                    FlowOrchestrator System Architecture                                   |
+----------------------------------------------------------------------------------------------------------+

+----------------------------------------------------------------------------------------------------------+
|                                           Core Domain (.NET 9)                                            |
+----------------------------------------------------------------------------------------------------------+
| FlowOrchestrator.Common                | FlowOrchestrator.Abstractions         | FlowOrchestrator.Domain |
| [Class Library]                        | [Class Library]                       | [Class Library]         |
|                                        |                                       |                         |
| - Shared utilities                     | - Core interfaces                     | - Domain models         |
| - Helper classes                       | - Abstract base interfaces            | - Entity definitions    |
| - Extension methods                    | - Contract definitions                | - Value objects         |
+----------------------------------------+---------------------------------------+-------------------------+
| FlowOrchestrator.Infrastructure.Common | FlowOrchestrator.Security.Common      |                         |
| [Class Library]                        | [Class Library]                       |                         |
|                                        |                                       |                         |
| - Shared infrastructure components     | - Security components                 |                         |
| - Cross-cutting concerns               | - Authentication                      |                         |
| - Common patterns                      | - Authorization                       |                         |
+----------------------------------------+---------------------------------------+-------------------------+

+----------------------------------------------------------------------------------------------------------+
|                                        Integration Domain (.NET 9)                                        |
+----------------------------------------------------------------------------------------------------------+
| FlowOrchestrator.IntegrationBase                                                                         |
| [Class Library]                                                                                          |
|                                                                                                          |
| - AbstractIntegrationComponent                                                                           |
| - Shared infrastructure for importers and exporters                                                      |
+----------------------------------------------------------------------------------------------------------+
|                                                                                                          |
|    +-----------------------------------+       +-----------------------------------+                     |
|    | FlowOrchestrator.ImporterBase    |       | FlowOrchestrator.ExporterBase    |                     |
|    | [Console Application]            |       | [Console Application]            |                     |
|    |                                   |       |                                   |                     |
|    | - AbstractImporterApplication    |       | - AbstractExporterApplication    |                     |
|    | - AbstractImporterService        |       | - AbstractExporterService        |                     |
|    | - IImporterService               |       | - IExporterService               |                     |
|    +-----------------------------------+       +-----------------------------------+                     |
|                     |                                           |                                        |
|                     v                                           v                                        |
|                +-------------+                              +-------------+                              |
|                | FileImporter|                              | FileExporter|                              |
|                | [Console    |                              | [Console    |                              |
|                | Application]|                              | Application]|                              |
|                +-------------+                              +-------------+                              |
+----------------------------------------------------------------------------------------------------------+

+----------------------------------------------------------------------------------------------------------+
|                                        Processing Domain (.NET 9)                                         |
+----------------------------------------------------------------------------------------------------------+
| FlowOrchestrator.ProcessingBase                                                                          |
| [Class Library]                                                                                          |
|                                                                                                          |
| - AbstractProcessingComponent                                                                            |
| - Shared infrastructure for processors                                                                   |
+----------------------------------------------------------------------------------------------------------+
|                                                                                                          |
|                          +-----------------------------------+                                           |
|                          | FlowOrchestrator.ProcessorBase   |                                           |
|                          | [Console Application]            |                                           |
|                          |                                   |                                           |
|                          | - AbstractProcessorApplication   |                                           |
|                          | - AbstractProcessorService       |                                           |
|                          | - IProcessorService              |                                           |
|                          +-----------------------------------+                                           |
|                                         |                                                                |
|                                         v                                                                |
|                              +------------------+                                                        |
|                              | JsonProcessor    |                                                        |
|                              | [Console         |                                                        |
|                              | Application]     |                                                        |
|                              +------------------+                                                        |
+----------------------------------------------------------------------------------------------------------+

+----------------------------------------------------------------------------------------------------------+
|                                        Management Domain (.NET 9)                                         |
+----------------------------------------------------------------------------------------------------------+
| FlowOrchestrator.EntityManagerBase                                                                       |
| [ASP.NET Core Web API]                                                                                   |
|                                                                                                          |
| - AbstractEntityManagerApplication                                                                       |
| - AbstractEntityController                                                                               |
| - AbstractManagerService<TService, TServiceId>                                                           |
| - Infrastructure Integration:                                                                            |
|   - MongoDB for persistent storage                                                                       |
|   - MassTransit for messaging                                                                            |
|   - OpenTelemetry for observability                                                                      |
+----------------------------------------------------------------------------------------------------------+
|                                                                                                          |
|    +-----------------------------------+    +-----------------------------------+                        |
|    | ImporterEntityManager            |    | ProcessorEntityManager           |                        |
|    | [ASP.NET Core Web API]           |    | [ASP.NET Core Web API]           |                        |
|    |                                   |    |                                   |                        |
|    | - CRUD Controller                 |    | - CRUD Controller                 |                        |
|    | - MongoDB Repository              |    | - MongoDB Repository              |                        |
|    | - MassTransit Integration         |    | - MassTransit Integration         |                        |
|    | - OpenTelemetry Integration       |    | - OpenTelemetry Integration       |                        |
|    | - Authentication/Authorization    |    | - Authentication/Authorization    |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
|                                                                                                          |
|    +-----------------------------------+    +-----------------------------------+                        |
|    | ExporterEntityManager            |    | FlowEntityManager                |                        |
|    | [ASP.NET Core Web API]           |    | [ASP.NET Core Web API]           |                        |
|    |                                   |    |                                   |                        |
|    | - CRUD Controller                 |    | - CRUD Controller                 |                        |
|    | - MongoDB Repository              |    | - MongoDB Repository              |                        |
|    | - MassTransit Integration         |    | - MassTransit Integration         |                        |
|    | - OpenTelemetry Integration       |    | - OpenTelemetry Integration       |                        |
|    | - Authentication/Authorization    |    | - Authentication/Authorization    |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
|                                                                                                          |
|    +-----------------------------------+    +-----------------------------------+                        |
|    | SourceEntityManager              |    | DestinationEntityManager         |                        |
|    | [ASP.NET Core Web API]           |    | [ASP.NET Core Web API]           |                        |
|    |                                   |    |                                   |                        |
|    | - CRUD Controller                 |    | - CRUD Controller                 |                        |
|    | - MongoDB Repository              |    | - MongoDB Repository              |                        |
|    | - MassTransit Integration         |    | - MassTransit Integration         |                        |
|    | - OpenTelemetry Integration       |    | - OpenTelemetry Integration       |                        |
|    | - Authentication/Authorization    |    | - Authentication/Authorization    |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
|                                                                                                          |
|    +-----------------------------------+    +-----------------------------------+                        |
|    | SourceAssignmentEntityManager    |    | DestinationAssignmentEntityManager|                        |
|    | [ASP.NET Core Web API]           |    | [ASP.NET Core Web API]           |                        |
|    |                                   |    |                                   |                        |
|    | - CRUD Controller                 |    | - CRUD Controller                 |                        |
|    | - MongoDB Repository              |    | - MongoDB Repository              |                        |
|    | - MassTransit Integration         |    | - MassTransit Integration         |                        |
|    | - OpenTelemetry Integration       |    | - OpenTelemetry Integration       |                        |
|    | - Authentication/Authorization    |    | - Authentication/Authorization    |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
|                                                                                                          |
|    +-----------------------------------+                                                                 |
|    | ScheduledFlowEntityManager       |                                                                 |
|    | [ASP.NET Core Web API]           |                                                                 |
|    |                                   |                                                                 |
|    | - CRUD Controller                 |                                                                 |
|    | - MongoDB Repository              |                                                                 |
|    | - MassTransit Integration         |                                                                 |
|    | - OpenTelemetry Integration       |                                                                 |
|    | - Authentication/Authorization    |                                                                 |
|    +-----------------------------------+                                                                 |
+----------------------------------------------------------------------------------------------------------+

+----------------------------------------------------------------------------------------------------------+
|                                        Execution Domain (.NET 9)                                          |
+----------------------------------------------------------------------------------------------------------+
| FlowOrchestrator.ExecutionBase                                                                           |
| [Class Library]                                                                                          |
|                                                                                                          |
| - AbstractExecutionComponent                                                                             |
| - Shared infrastructure for execution components                                                         |
+----------------------------------------------------------------------------------------------------------+
|                                                                                                          |
|    +-----------------------------------+    +-----------------------------------+                        |
|    | FlowOrchestrator.Orchestrator    |    | FlowOrchestrator.MemoryManager   |                        |
|    | [Worker Service]                 |    | [Worker Service]                 |                        |
|    |                                   |    |                                   |                        |
|    | - Flow execution coordination     |    | - Memory management              |                        |
|    | - Branch management               |    | - Resource allocation            |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
|                                                                                                          |
|    +-----------------------------------+    +-----------------------------------+                        |
|    | FlowOrchestrator.BranchManager   |    | FlowOrchestrator.TaskScheduler   |                        |
|    | [Worker Service]                 |    | [Worker Service]                 |                        |
|    |                                   |    |                                   |                        |
|    | - Branch execution coordination   |    | - Task scheduling                |                        |
|    | - Parallel processing             |    | - Flow triggering                |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
+----------------------------------------------------------------------------------------------------------+

+----------------------------------------------------------------------------------------------------------+
|                                      Observability Domain (.NET 9)                                        |
+----------------------------------------------------------------------------------------------------------+
| FlowOrchestrator.ObservabilityBase                                                                       |
| [Class Library]                                                                                          |
|                                                                                                          |
| - AbstractObservabilityComponent                                                                         |
| - Shared infrastructure for observability components                                                     |
+----------------------------------------------------------------------------------------------------------+
|                                                                                                          |
|    +-----------------------------------+    +-----------------------------------+                        |
|    | FlowOrchestrator.StatisticsService|    | FlowOrchestrator.MonitoringFramework|                     |
|    | [Worker Service]                 |    | [ASP.NET Core Web API]           |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
|                                                                                                          |
|    +-----------------------------------+    +-----------------------------------+                        |
|    | FlowOrchestrator.AlertingSystem  |    | FlowOrchestrator.AnalyticsEngine |                        |
|    | [Worker Service]                 |    | [Worker Service]                 |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
+----------------------------------------------------------------------------------------------------------+

+----------------------------------------------------------------------------------------------------------+
|                                      Infrastructure Domain (.NET 9)                                       |
+----------------------------------------------------------------------------------------------------------+
|    +-----------------------------------+    +-----------------------------------+                        |
|    | FlowOrchestrator.Data.MongoDB    |    | FlowOrchestrator.Messaging.MassTransit|                   |
|    | [Class Library]                  |    | [Class Library]                  |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
|                                                                                                          |
|    +-----------------------------------+    +-----------------------------------+                        |
|    | FlowOrchestrator.Scheduling.Quartz|    | FlowOrchestrator.Telemetry.OpenTelemetry|                |
|    | [Class Library]                  |    | [Class Library]                  |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
+----------------------------------------------------------------------------------------------------------+

+----------------------------------------------------------------------------------------------------------+
|                                           Test Projects (.NET 9)                                          |
+----------------------------------------------------------------------------------------------------------+
|    +-----------------------------------+    +-----------------------------------+                        |
|    | Unit Tests                        |    | Integration Tests                |                        |
|    | [xUnit Test Project]             |    | [xUnit Test Project]             |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
|                                                                                                          |
|    +-----------------------------------+    +-----------------------------------+                        |
|    | System Tests                      |    | Performance Tests                |                        |
|    | [xUnit Test Project]             |    | [xUnit Test Project]             |                        |
|    +-----------------------------------+    +-----------------------------------+                        |
+----------------------------------------------------------------------------------------------------------+
```

## Key Relationships and Inheritance Hierarchies

### Core Domain Interfaces
```
IServiceBase
├── IImporterService
├── IProcessorService
└── IExporterService
```

### Entity Interfaces
```
IEntity
├── IFlowEntity
├── IProcessingChainEntity
├── ISourceEntity
├── IDestinationEntity
├── ISourceAssignmentEntity
├── IDestinationAssignmentEntity
├── IScheduledFlowEntity
└── ITaskSchedulerEntity
```

### Abstract Service Classes
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

### Abstract Application Classes
```
AbstractApplicationBase
├── AbstractImporterApplication
│   └── FileImporterApplication
│
├── AbstractProcessorApplication
│   └── JsonProcessorApplication
│
└── AbstractExporterApplication
    └── FileExporterApplication
```

### Entity Manager Classes
```
AbstractEntityManagerApplication
├── AbstractManagerService<TService, TServiceId>
│   ├── ImporterServiceManager
│   ├── ProcessorServiceManager
│   ├── ExporterServiceManager
│   ├── SourceEntityManager
│   ├── DestinationEntityManager
│   ├── SourceAssignmentEntityManager
│   ├── DestinationAssignmentEntityManager
│   ├── TaskSchedulerEntityManager
│   └── ScheduledFlowEntityManager
```

### Entity Classes
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

## Component Interactions

### Registration Flow
```
+-------------------+                 +-------------------+                 +-------------------+
|                   |                 |                   |                 |                   |
|  Component        |     Publish     |  Message Bus      |     Consume     |  Entity Manager   |
|  (Importer,       | --------------> |  (MassTransit)    | --------------> |  (ImporterEntity  |
|   Processor,      |  Registration   |                   |  Registration   |   Manager, etc.)  |
|   Exporter)       |    Message      |                   |    Message      |                   |
|                   |                 |                   |                 |                   |
+-------------------+                 +-------------------+                 +-------------------+
```

### Flow Execution
```
+-------------------+                 +-------------------+                 +-------------------+
|                   |                 |                   |                 |                   |
|  Orchestrator     |     Publish     |  Message Bus      |     Consume     |  Importer         |
|                   | --------------> |  (MassTransit)    | --------------> |                   |
|                   |  Import Command |                   |  Import Command |                   |
|                   |                 |                   |                 |                   |
+-------------------+                 +-------------------+                 +-------------------+
        ^                                     ^                                     |
        |                                     |                                     |
        |                                     |                                     v
        |                                     |                             +-------------------+
        |                                     |                             |                   |
        |                                     |                             |  Memory Manager   |
        |                                     |                             |                   |
        |                                     |                             +-------------------+
        |                                     |                                     |
        |                                     |                                     v
+-------------------+                 +-------------------+                 +-------------------+
|                   |                 |                   |                 |                   |
|  Branch Manager   |     Publish     |  Message Bus      |     Consume     |  Processor        |
|                   | <-------------- |  (MassTransit)    | <-------------- |                   |
|                   | Import Complete |                   | Process Command |                   |
|                   |                 |                   |                 |                   |
+-------------------+                 +-------------------+                 +-------------------+
```

### Validation Flow
```
+-------------------+                 +-------------------+                 +-------------------+
|                   |                 |                   |                 |                   |
|  Flow Entity      |     Validate    |  Processing Chain |     Validate    |  Processor Entity |
|  Manager          | --------------> |  Entity Manager   | --------------> |  Manager          |
|                   |  Chain          |                   |  Processors     |                   |
|                   |  Compatibility  |                   |  Compatibility  |                   |
+-------------------+                 +-------------------+                 +-------------------+
```

### Observability Flow
```
+-------------------+                 +-------------------+                 +-------------------+
|                   |                 |                   |                 |                   |
|  Component        |     Report      |  OpenTelemetry    |     Collect     |  Monitoring       |
|  (Importer,       | --------------> |  Collector        | --------------> |  Framework        |
|   Processor,      |  Telemetry      |                   |  Telemetry      |                   |
|   Exporter)       |  Data           |                   |  Data           |                   |
|                   |                 |                   |                 |                   |
+-------------------+                 +-------------------+                 +-------------------+
```