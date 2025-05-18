# 1. System Overview

## 1.1 High-Level Architecture

FlowOrchestrator follows a modular service-oriented architecture with the following key characteristics:
- Clear separation of concerns across specialized components
- Message-based communication between services
- Shared memory for efficient data transfer
- Centralized orchestration with distributed execution
- Parallel processing through isolated branches
- Comprehensive observability through integrated telemetry
- Version-controlled components for system evolution

The high-level architecture can be visualized as follows:

```
┌───────────────────────────────────────────────────────────────────────────┐
│                         FlowOrchestrator System                            │
│                                                                           │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐ │
│  │  External   │    │  Importer   │    │ Processing  │    │  Exporter   │ │
│  │   Source    │───▶│  Service    │───▶│   Chain     │───▶│  Service    │ │
│  └─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘ │
│                           │                  │                  │         │
│                           │                  │                  │         │
│                           ▼                  ▼                  ▼         │
│                    ┌─────────────────────────────────────────────────┐   │
│                    │               Shared Memory                      │   │
│                    └─────────────────────────────────────────────────┘   │
│                                        ▲                                  │
│                                        │                                  │
│  ┌─────────────┐               ┌───────────────┐               ┌────────┐│
│  │    Task     │◀──────────────│  Orchestrator │───────────────▶│ Stats  ││
│  │  Scheduler  │───────────────▶│    Service   │               │Service ││
│  └─────────────┘               └───────────────┘               └────────┘│
│                                                                           │
└───────────────────────────────────────────────────────────────────────────┘
```

### Event-Driven Orchestration as Core Pattern

The FlowOrchestrator system implements event-driven orchestration as its core architectural pattern:

- **Event-Based Flow Progression**: Flow execution advances through a series of events
- **Message-Driven Communication**: Components communicate through well-defined messages
- **Centralized Orchestration**: The Orchestrator Service coordinates event flow
- **Decentralized Processing**: Individual services handle specific processing responsibilities
- **Event-Based State Transitions**: System state changes in response to specific events

## 1.2 Core Principles

### 1.2.1 Stateless Design
- Services do not depend on previous states
- Enhances reliability and scalability
- Exception: memory services maintain state
- Orchestrator maintains state of flow execution across stateless processors
- Services can be restarted without loss of system integrity
- Enables horizontal scaling and load balancing
- Simplifies recovery from service failures
- Promotes clean separation of concerns

### 1.2.2 Message-Based Communication
- Services connect through a shared message pipeline
- Messages are sent and received between components
- Messages contain routing information for data placement in shared memory
- Task Scheduler directly initiates data import via messages to the Importer Service
- Orchestrator Service provides initial execution context to Task Scheduler
- Orchestration logic is centralized in the Orchestrator Service after import completion
- Provides loose coupling between system parts
- Supports various connection protocols between system and external sources/destinations

### 1.2.3 Shared Memory Model
- Central cache serves as shared memory
- Services write to and read from the cache based on orchestrator instructions
- Memory locations are assigned dynamically by the Orchestrator Service
- Hierarchical naming pattern ensures unique and traceable data storage
- Used for passing data between processing steps
- Enables efficient data transfer without serialization/deserialization overhead
- Branch-specific memory locations ensure isolation between parallel execution paths

### 1.2.4 Data Storage Strategy
- Information stored in key-value dictionary
- Data structures mapped by hierarchical naming convention
- Format: {executionId}:{flowId}:{stepType}:{branchPath}:{stepId}:{dataType}
- Enables efficient access and retrieval
- Maintains clear separation between different flow executions
- Facilitates data lifecycle management and cleanup
- Supports branch isolation and merge operations

### 1.2.5 Version Control
- All system components maintain version information
- Version identifiers follow semantic versioning (MAJOR.MINOR.PATCH)
- Component versions are immutable once created
- Changes to components require creating new versions
- Version compatibility is enforced during flow construction and execution
- Version history is maintained for all components
- Version status transitions (ACTIVE → DEPRECATED → ARCHIVED) are tracked

## 1.3 System Benefits

### 1.3.1 Scalability
- Stateless services scale horizontally
- Can handle increased processing loads
- Components can be distributed across resources
- Dynamic service allocation based on workload
- Parallel branch execution improves throughput
- Independent scaling of different system components
- Efficient resource utilization through dynamic allocation
- Adaptable to varying load patterns

### 1.3.2 Flexibility
- Components can be added, removed, or modified
- Changes to one component don't affect others
- Supports evolving business requirements
- Message-based orchestration enables dynamic flow adjustments
- Branch-based processing enables complex transformation patterns
- Versioning allows controlled evolution of system components
- Multiple protocol support through specialized implementations
- Configuration-driven behavior modification

### 1.3.3 Reliability
- Failures in one service don't propagate
- Fault isolation improves system stability
- Services can recover independently
- Branch isolation contains failures to specific execution paths
- Orchestrator can monitor and manage flow execution
- Retry and recovery mechanisms enhance resilience
- Standardized error classification enables consistent handling
- Version control prevents incompatible component combinations
- Comprehensive observability supports early issue detection

### 1.3.4 Maintainability
- Clear separation of concerns
- System is easier to debug and update
- Components have well-defined responsibilities
- Centralized orchestration simplifies flow management
- Standardized interfaces reduce cognitive load
- Branch-based organization provides logical grouping of processing steps
- Service lifecycle management ensures consistent component states
- Versioning enables controlled system evolution over time
- Comprehensive documentation and observability

### 1.3.5 Reusability
- Entities and services reusable across flows with matching protocol requirements
- Protocol-specific importers and exporters enable specialization and optimization
- Common configurations can be templated for each protocol type
- Processors can be reused across multiple branches with unique step identifiers
- Reduces development effort for new flows with similar connection requirements
- Version compatibility matrix ensures proper component reuse
- Service registries facilitate discovery of existing components
- Standardized interfaces promote consistent implementations

## 1.4 System Boundaries and External Interfaces

### 1.4.1 Integration Points
- External Source Systems: Provide data input to the system
- External Destination Systems: Receive processed data from the system
- Management Systems: Configure and monitor the FlowOrchestrator
- Security Systems: Provide authentication and authorization
- Monitoring Systems: Receive telemetry and alerts


### 1.4.3 Client Interfaces
- Administrative API: For system configuration and management
- Monitoring API: For observability and telemetry access
- Configuration API: For flow definition and management
- Execution API: For controlling flow execution
- Reporting API: For accessing execution statistics and results

### 1.4.4 Deployment Boundaries
- Core System Components: Required for minimal functionality
- Extension Components: Optional for specific use cases
- Management Tools: For system administration
- Development Tools: For flow creation and testing
- Integration Adapters: For connecting to external systems

## 1.5 Solution Structure

### 1.5.1 Single Solution Approach

The FlowOrchestrator system will be implemented as a single Visual Studio solution containing multiple projects. This monorepo architecture provides several significant benefits:

1. **Centralized Development**: All code is maintained in a single repository, simplifying version control, continuous integration, and deployment processes.

2. **Simplified Dependencies**: Project references between components are managed directly within the solution, ensuring consistent versioning and eliminating version conflicts.

3. **Shared Code Reuse**: Common libraries, interfaces, and abstract classes can be easily shared across all microservices without duplication.

4. **Coordinated Testing**: Integration and system testing can be performed across all components simultaneously, ensuring proper interaction between services.

5. **Unified Build Process**: The entire system can be built, tested, and deployed from a single pipeline, streamlining DevOps processes.


#### Infrastructure Projects
Infrastructure projects provide the technical foundation for the system:

- **MongoDB Integration**: Data persistence infrastructure
- **Hazelcast Integration**: In-memory data grid infrastructure
- **MassTransit Integration**: Messaging infrastructure
- **Quartz.NET Integration**: Scheduling infrastructure
- **OpenTelemetry Integration**: Observability infrastructure

### 1.5.4 Project Dependencies

Project dependencies will follow strict layering principles to prevent circular dependencies:

1. **Core Projects**: Have no dependencies on other FlowOrchestrator projects
2. **Domain Service Projects**: Depend on Core projects and Infrastructure projects
3. **Infrastructure Projects**: Depend only on Core projects
4. **Test Projects**: Depend on the projects they are testing and test infrastructure

Dependencies between domain service projects will be minimized, with communication primarily occurring through well-defined interfaces and messaging.

### 1.5.5 Deployment Independence

Despite being part of a single solution, each microservice will maintain deployment independence:

1. **Containerization**: Each microservice will be containerized individually
2. **Configuration**: Each microservice will have its own configuration
3. **Versioning**: Each microservice will be versioned independently
4. **Scaling**: Each microservice can be scaled independently
5. **Deployment**: Each microservice can be deployed independently

This approach maintains the benefits of microservice architecture while simplifying development and ensuring consistency.

### 1.5.6 Microservice Boundaries

To maintain clear boundaries between microservices despite the single solution:

1. **Explicit Interfaces**: All inter-service communication goes through explicit interfaces
2. **Message-Based Communication**: Services communicate via messages, not direct method calls
3. **Database Isolation**: Each service has logical isolation in the database
4. **No Shared State**: Services do not share mutable state except through the defined memory manager
5. **Independent Configuration**: Each service has its own configuration

These boundaries ensure that the system maintains the benefits of a microservice architecture while simplifying development through the single solution approach.

## 3.3 Processing Domain

The Processing Domain contains components responsible for data transformation and processing.

### 3.3.1 Processor Console Application
- **Definition**: Abstract base for console application defining the data transformation engine interface
- **Purpose**: Defines the contract for transforming, enriching, and processing information
- **Responsibilities**: 
  - Serves as a base class/interface from which concrete processor implementations are derived
  - Defines standard methods and properties that all processors must implement
  - Provides standardized interface for data transformation
  - Processes data based on step-specific configuration
  - Operates independently of branch context (stateless)
  - Can be used multiple times within a flow with unique step identifiers
- Use open telemetry infustructure services- logs is mandatory but the metrics and  traces are optional.
- Use Masstransit message bus infrustructure services
-have scheme in and out definition for verify compatibility.
- Every restart the console will send to ProcessorEntityManager a creation/registration message.
The manager will create Processor entity if not exist and save all the relevant information.
