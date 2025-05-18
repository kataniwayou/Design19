# Concrete Processor Implementation Guide

## Overview

Concrete processor implementations in the FlowOrchestrator system inherit from the AbstractProcessorApplication and focus solely on implementing specific processing logic and providing processor metadata. This guide outlines the structure and best practices for implementing concrete processors.

## Project Structure

A typical concrete processor project should have the following structure:

```
FlowOrchestrator.JsonProcessor/
├── JsonProcessor.cs                        # Main implementation class
├── Transformation/                         # Processor-specific logic
│   ├── JsonTransformationEngine.cs
│   └── JsonPathEvaluator.cs
├── Schema/
│   ├── InputSchemaDefinition.cs            # Input schema details
│   └── OutputSchemaDefinition.cs           # Output schema details
└── Properties/
    └── launchSettings.json                 # Launch settings
```

## Implementation Pattern

### Main Processor Class

```csharp
using FlowOrchestrator.ProcessorBase;
using FlowOrchestrator.Domain.Models;
using FlowOrchestrator.Domain.Schema;

namespace FlowOrchestrator.JsonProcessor
{
    public class JsonProcessor : AbstractProcessorApplication
    {
        // Identity properties - these define the processor entity
        public override string ProcessorId => "JSON-PROC-001";
        public override string ProcessorName => "JSON Transformation Processor";
        public override string ProcessorType => "TRANSFORMATION";
        public override string Version => "1.0.0";
        
        // Core processing logic - this is the main method to implement
        protected override ProcessingResult Process(ProcessParameters parameters, ExecutionContext context)
        {
            // Get the transformation engine
            var transformationEngine = new JsonTransformationEngine();
            
            // Transform the data
            var transformedData = transformationEngine.Transform(
                parameters.InputData, 
                parameters.Configuration.GetValue<string>("transformationRules")
            );
            
            // Create and return the result
            var result = new ProcessingResult
            {
                TransformedData = transformedData,
                TransformationMetadata = new Metadata
                {
                    ProcessorId = ProcessorId,
                    ProcessorVersion = Version,
                    ProcessingTimestamp = DateTime.UtcNow,
                    // Additional metadata...
                }
            };
            
            return result;
        }
        
        // Schema definitions
        protected override SchemaDefinition GetInputSchema()
        {
            return new InputSchemaDefinition().GetSchema();
        }
        
        protected override SchemaDefinition GetOutputSchema()
        {
            return new OutputSchemaDefinition().GetSchema();
        }
        
        // Capabilities
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
}
```

### Schema Definition Classes

```csharp
namespace FlowOrchestrator.JsonProcessor.Schema
{
    public class InputSchemaDefinition
    {
        public SchemaDefinition GetSchema()
        {
            return new SchemaDefinition
            {
                Name = "JsonProcessorInput",
                Version = "1.0.0",
                Fields = new List<SchemaField>
                {
                    new SchemaField 
                    { 
                        Name = "sourceData", 
                        Type = "json", 
                        Required = true,
                        Description = "Source JSON data to transform",
                        ValidationRules = new Dictionary<string, object>
                        {
                            { "minLength", 2 },
                            { "maxLength", 10485760 } // 10MB
                        }
                    },
                    new SchemaField 
                    { 
                        Name = "transformationContext", 
                        Type = "object", 
                        Required = false,
                        Description = "Optional context for transformation"
                    }
                }
            };
        }
    }
}
```

## Key Implementation Aspects

### 1. Identity Properties

The identity properties define the processor entity that will be created in the Processor Entity Manager:

```csharp
public override string ProcessorId => "JSON-PROC-001";
public override string ProcessorName => "JSON Transformation Processor";
public override string ProcessorType => "TRANSFORMATION";
public override string Version => "1.0.0";
```

These properties should be carefully chosen and documented:
- **ProcessorId**: Unique identifier for the processor
- **ProcessorName**: Human-readable name
- **ProcessorType**: Classification (e.g., TRANSFORMATION, VALIDATION, ENRICHMENT)
- **Version**: Semantic version (MAJOR.MINOR.PATCH)

### 2. Process Method

The Process method is the core of the concrete implementation:

```csharp
protected override ProcessingResult Process(ProcessParameters parameters, ExecutionContext context)
{
    // Implementation...
}
```

This method should:
- Focus on the specific transformation logic
- Handle the input data according to the processor's purpose
- Create and return a properly structured ProcessingResult
- Use processor-specific helper classes for complex logic

### 3. Schema Definitions

Schema definitions are crucial for compatibility validation:

```csharp
protected override SchemaDefinition GetInputSchema()
{
    // Implementation...
}

protected override SchemaDefinition GetOutputSchema()
{
    // Implementation...
}
```

These methods should:
- Define clear, well-structured schemas
- Include all required fields with proper types
- Document field purposes and constraints
- Include validation rules where appropriate

### 4. Capabilities

Capabilities help the entity managers understand what the processor can do:

```csharp
protected override List<string> GetCapabilities()
{
    return new List<string>
    {
        "JSON_PATH_QUERY",
        "JSON_TRANSFORMATION",
        "SCHEMA_VALIDATION"
    };
}
```

Capabilities should:
- Follow the system's capability taxonomy
- Be accurate and not overstated
- Help with compatibility validation

## Best Practices

1. **Focus on Core Logic**: Implement only the specific processing logic and metadata.

2. **Avoid Infrastructure Code**: Never include infrastructure setup or message handling code.

3. **Separate Concerns**: Use helper classes for complex processing logic.

4. **Document Thoroughly**: Include XML documentation for all public members.

5. **Version Carefully**: Follow semantic versioning principles.

6. **Test Comprehensively**: Create unit tests for the Process method and schema definitions.

7. **Error Handling**: Let the base class handle infrastructure errors; only handle processing-specific errors.

8. **Configuration**: Define clear configuration parameters and validate them properly.

This implementation pattern ensures that concrete processors remain focused on their specific processing logic while inheriting all the infrastructure and common functionality from the abstract base class.
