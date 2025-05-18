# Component Interactions and Relationships

This document describes the interactions and relationships between the various components of the FlowOrchestrator system.

## 1. Registration Flow

The registration flow is the process by which components register themselves with their respective Entity Managers.

### 1.1 Component Self-Registration

```
+-------------------+                 +-------------------+                 +-------------------+
|                   |                 |                   |                 |                   |
|  Component        |     Publish     |  Message Bus      |     Consume     |  Entity Manager   |
|  (Importer,       | --------------> |  (MassTransit)    | --------------> |  (ImporterEntity  |
|   Processor,      |  Registration   |                   |  Registration   |   Manager, etc.)  |
|   Exporter)       |    Message      |                   |    Message      |                   |
|                   |                 |                   |                 |                   |
+-------------------+                 +-------------------+                 +-------------------+
        |                                                                           |
        |                                                                           |
        |                                                                           v
        |                                                                  +-------------------+
        |                                                                  |                   |
        |                                                                  |  MongoDB          |
        |                                                                  |  (Entity Storage) |
        |                                                                  |                   |
        |                                                                  +-------------------+
        |
        v
+-------------------+
|                   |
|  OpenTelemetry    |
|  (Observability)  |
|                   |
+-------------------+
```

### 1.2 Registration Sequence

1. Component starts up and configures its services
2. Component creates a registration message with its metadata:
   - Component ID
   - Component Name
   - Component Type
   - Version
   - Protocol (for Importers/Exporters)
   - Capabilities
   - Schema Information
3. Component publishes the registration message to the message bus
4. Entity Manager consumes the registration message
5. Entity Manager validates the registration information
6. Entity Manager stores the component metadata in MongoDB
7. Entity Manager sends a registration acknowledgment message
8. Component receives the acknowledgment and updates its status

## 2. Flow Execution

The flow execution process involves multiple components working together to process data.

### 2.1 Flow Execution Overview

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
        |                                     ^                                     |
        |                                     |                                     |
        v                                     |                                     v
+-------------------+                         |                             +-------------------+
|                   |                         |                             |                   |
|  Parallel         |                         |                             |  Memory Manager   |
|  Branch           |                         |                             |                   |
|  Execution        |                         |                             +-------------------+
|                   |                         |                                     |
+-------------------+                         |                                     v
        |                                     |                             +-------------------+
        |                                     |                             |                   |
        v                                     |                             |  Exporter         |
+-------------------+                 +-------------------+                 |                   |
|                   |     Publish     |                   |     Consume     |                   |
|  Branch Manager   | --------------> |  Message Bus      | --------------> |                   |
|                   |  Export Command |  (MassTransit)    |  Export Command |                   |
|                   |                 |                   |                 |                   |
+-------------------+                 +-------------------+                 +-------------------+
```

### 2.2 Flow Execution Sequence

1. Orchestrator receives a flow execution request
2. Orchestrator retrieves flow metadata from FlowEntityManager
3. Orchestrator sends an import command to the Importer
4. Importer processes the import command and imports data
5. Importer stores the imported data in Memory Manager
6. Importer sends an import complete message
7. Branch Manager receives the import complete message
8. Branch Manager determines the next processors to execute
9. Branch Manager sends process commands to the Processors
10. Processors process the data and store results in Memory Manager
11. Processors send process complete messages
12. Branch Manager receives process complete messages
13. Branch Manager determines when all branches are complete
14. Branch Manager sends export commands to the Exporters
15. Exporters process the export commands and export data
16. Exporters send export complete messages
17. Orchestrator receives export complete messages
18. Orchestrator completes the flow execution

## 3. Validation Flow

The validation flow is the process by which Entity Managers validate the compatibility of components.

### 3.1 Validation Overview

```
+-------------------+                 +-------------------+                 +-------------------+
|                   |                 |                   |                 |                   |
|  Flow Entity      |     Validate    |  Processing Chain |     Validate    |  Processor Entity |
|  Manager          | --------------> |  Entity Manager   | --------------> |  Manager          |
|                   |  Chain          |                   |  Processors     |                   |
|                   |  Compatibility  |                   |  Compatibility  |                   |
+-------------------+                 +-------------------+                 +-------------------+
        |                                                                           ^
        |                                                                           |
        v                                                                           |
+-------------------+                 +-------------------+                         |
|                   |                 |                   |                         |
|  Importer Entity  |     Validate    |  Source Assignment|                         |
|  Manager          | <-------------- |  Entity Manager   |                         |
|                   |  Importer       |                   |                         |
|                   |  Compatibility  |                   |                         |
+-------------------+                 +-------------------+                         |
        ^                                     ^                                     |
        |                                     |                                     |
        |                                     |                                     |
        |                                     |                                     |
+-------------------+                 +-------------------+                         |
|                   |                 |                   |                         |
|  Exporter Entity  |     Validate    |  Destination      |                         |
|  Manager          | <-------------- |  Assignment       |                         |
|                   |  Exporter       |  Entity Manager   |                         |
|                   |  Compatibility  |                   |                         |
+-------------------+                 +-------------------+                         |
        ^                                                                           |
        |                                                                           |
        |                                                                           |
+-------------------+                                                               |
|                   |                                                               |
|  Scheduled Flow   |                                                               |
|  Entity Manager   | ------------------------------------------------------->------+
|                   |  Validate All Component Compatibility
|                   |
+-------------------+
```

### 3.2 Validation Responsibilities

1. **ProcessingChainEntityManager**:
   - Validates processor compatibility within a chain
   - Ensures output schema of one processor matches input schema of the next
   - Verifies that branch paths are properly defined
   - Ensures no cycles exist in the processing chain

2. **FlowEntityManager**:
   - Validates importer/exporter/chain compatibility
   - Ensures importer output schema matches first processor input schema
   - Ensures last processor output schema matches exporter input schema
   - Verifies that all branches terminate at exporters
   - Ensures the flow has exactly one importer
   - Validates that branch paths are properly defined

3. **SourceAssignmentEntityManager**:
   - Verifies source-importer compatibility
   - Ensures importer can handle the source protocol
   - Validates source configuration against importer capabilities

4. **DestinationAssignmentEntityManager**:
   - Verifies destination-exporter compatibility
   - Ensures exporter can handle the destination protocol
   - Validates destination configuration against exporter capabilities

5. **ScheduledFlowEntityManager**:
   - Verifies compatibility between source/destination assignments and flow entities
   - Ensures all components in the scheduled flow are compatible
   - Validates scheduling parameters

## 4. Observability Flow

The observability flow is the process by which components report telemetry data.

### 4.1 Observability Overview

```
+-------------------+                 +-------------------+                 +-------------------+
|                   |                 |                   |                 |                   |
|  Component        |     Report      |  OpenTelemetry    |     Collect     |  Monitoring       |
|  (Importer,       | --------------> |  Collector        | --------------> |  Framework        |
|   Processor,      |  Telemetry      |                   |  Telemetry      |                   |
|   Exporter)       |  Data           |                   |  Data           |                   |
|                   |                 |                   |                 |                   |
+-------------------+                 +-------------------+                 +-------------------+
                                                                                    |
                                                                                    |
                                                                                    v
                                                                           +-------------------+
                                                                           |                   |
                                                                           |  Statistics       |
                                                                           |  Service          |
                                                                           |                   |
                                                                           +-------------------+
                                                                                    |
                                                                                    |
                                      +-------------------+                         |
                                      |                   |                         |
                                      |  Alerting         |                         |
                                      |  System           | <-----------------------+
                                      |                   |
                                      +-------------------+
                                                |
                                                |
                                                v
                                      +-------------------+
                                      |                   |
                                      |  Analytics        |
                                      |  Engine           |
                                      |                   |
                                      +-------------------+
```

### 4.2 Observability Data Types

1. **Metrics**:
   - Performance metrics (throughput, latency, etc.)
   - Resource utilization metrics (CPU, memory, etc.)
   - Business metrics (flow executions, data volumes, etc.)

2. **Traces**:
   - End-to-end flow execution traces
   - Component-specific operation traces
   - Cross-component interaction traces

3. **Logs**:
   - Application logs
   - Error logs
   - Audit logs

4. **Events**:
   - Flow execution events
   - Component lifecycle events
   - Error events
