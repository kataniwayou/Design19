# Entity Manager Validation Responsibilities

## Overview

The FlowOrchestrator system implements a multi-layered validation approach at design time to ensure compatibility and correctness before runtime. Each entity manager has specific validation responsibilities focused on different aspects of the system.

## Validation Responsibilities by Entity Manager

### 1. Processing Chain Entity Manager

**Primary Responsibility**: Verifies the compatibility of processor entities within processing chains.

**Specific Validations**:
- Validates schema compatibility between connected processors (output schema of processor N matches input schema of processor N+1)
- Verifies data type compatibility throughout the processing chain
- Validates that the chain forms a valid directed acyclic graph (no cycles)
- Ensures that all processors in the chain are properly connected
- Verifies that branch paths are properly defined
- Checks that processor capabilities match the requirements of their position in the chain

### 2. Flow Entity Manager

**Primary Responsibility**: Verifies compatibility between processing chains, importers, and exporters.

**Specific Validations**:
- Validates compatibility between importer output and first processor input schemas
- Ensures compatibility between last processor output and exporter input schemas
- Validates that all processing chains within the flow have compatible schemas
- Checks that merge points at exporters have compatible input schemas from different branches
- Ensures the flow has exactly one importer
- Verifies that all branches terminate at exporters
- Validates that branch paths are properly defined
- Ensures no cycles exist in the flow graph
- Verifies that branches only merge at exporters

### 3. Source Assignment Entity Manager

**Primary Responsibility**: Verifies compatibility between source entities and importer entities.

**Specific Validations**:
- Ensures source protocol compatibility with importer capabilities
- Verifies that source data format is compatible with importer processing requirements
- Validates that source connection parameters are compatible with importer configuration
- Validates that the Source Entity version is compatible with the Importer Service version
- Ensures that protocol versions are compatible between source and importer
- Verifies that authentication mechanisms are compatible

### 4. Destination Assignment Entity Manager

**Primary Responsibility**: Verifies compatibility between destination entities and exporter entities.

**Specific Validations**:
- Ensures exporter protocol compatibility with destination requirements
- Verifies that exporter output format is compatible with destination expectations
- Validates that destination connection parameters are compatible with exporter configuration
- Confirms that exporter merge capabilities match the flow requirements
- Validates that the Destination Entity version is compatible with the Exporter Service version
- Ensures that protocol versions are compatible between exporter and destination
- Verifies that authentication mechanisms are compatible

### 5. Scheduled Flow Entity Manager

**Primary Responsibility**: Performs final validation of the complete flow from source to destination.

**Specific Validations**:
- Verifies compatibility across source assignment, flow, and destination assignment
- Ensures that all components referenced in the scheduled flow have compatible versions
- Validates that the complete data flow path from source to destination is compatible
- Confirms that all version dependencies and constraints are satisfied
- Validates that the source assignment is compatible with the flow's importer
- Ensures that the flow's exporters are compatible with the destination assignment
- Verifies that all required parameters for end-to-end execution are provided
- Checks that scheduling parameters are valid and consistent

## Validation Timing

All validations occur at design time, not at runtime:

1. **Component Registration**: Initial validation when components are registered
2. **Entity Creation**: Validation when entities are created or updated
3. **Relationship Establishment**: Validation when relationships between entities are established
4. **Flow Construction**: Validation during flow construction
5. **Flow Scheduling**: Final validation before a flow is scheduled for execution

## Benefits of Design-Time Validation

1. **Early Error Detection**: Incompatibilities are detected before runtime
2. **Reduced Runtime Failures**: Prevents execution of incompatible flows
3. **Improved Developer Experience**: Provides immediate feedback during design
4. **System Integrity**: Ensures that only valid configurations enter the system
5. **Operational Stability**: Reduces the risk of runtime errors due to incompatible components

This multi-layered validation approach ensures that incompatibilities are detected and prevented during design time, significantly reducing the risk of runtime errors.
