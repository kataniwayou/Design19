using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.ProcessorEntityManager.Validators;
using Xunit;

namespace FlowOrchestrator.ProcessorEntityManager.Tests.Validators;

public class ProcessorValidatorTests
{
    private readonly ProcessorValidator _validator;

    public ProcessorValidatorTests()
    {
        _validator = new ProcessorValidator();
    }

    [Fact]
    public void Validate_WithValidEntity_ReturnsValidResult()
    {
        // Arrange
        var entity = new ProcessorEntity
        {
            Name = "Test Processor",
            ProcessorType = "JsonProcessor",
            Version = "1.0.0",
            InputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "InputField",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            OutputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "OutputField",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            Capabilities = new List<string> { "JsonProcessing" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithMissingName_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new ProcessorEntity
        {
            ProcessorType = "JsonProcessor",
            Version = "1.0.0",
            InputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "InputField",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            OutputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "OutputField",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            Capabilities = new List<string> { "JsonProcessing" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.Name));
    }

    [Fact]
    public void Validate_WithMissingProcessorType_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new ProcessorEntity
        {
            Name = "Test Processor",
            Version = "1.0.0",
            InputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "InputField",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            OutputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "OutputField",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            Capabilities = new List<string> { "JsonProcessing" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.ProcessorType));
    }

    [Fact]
    public void Validate_WithMissingInputSchema_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new ProcessorEntity
        {
            Name = "Test Processor",
            ProcessorType = "JsonProcessor",
            Version = "1.0.0",
            OutputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "OutputField",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            Capabilities = new List<string> { "JsonProcessing" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.InputSchema));
    }

    [Fact]
    public void Validate_WithMissingOutputSchema_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new ProcessorEntity
        {
            Name = "Test Processor",
            ProcessorType = "JsonProcessor",
            Version = "1.0.0",
            InputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "InputField",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            Capabilities = new List<string> { "JsonProcessing" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.OutputSchema));
    }

    [Fact]
    public void Validate_WithMissingCapabilities_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new ProcessorEntity
        {
            Name = "Test Processor",
            ProcessorType = "JsonProcessor",
            Version = "1.0.0",
            InputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "InputField",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            OutputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "OutputField",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.Capabilities));
    }
}
