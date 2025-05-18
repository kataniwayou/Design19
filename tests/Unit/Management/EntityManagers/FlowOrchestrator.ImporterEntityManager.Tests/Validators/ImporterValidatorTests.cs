using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.ImporterEntityManager.Validators;
using Xunit;

namespace FlowOrchestrator.ImporterEntityManager.Tests.Validators;

public class ImporterValidatorTests
{
    private readonly ImporterValidator _validator;

    public ImporterValidatorTests()
    {
        _validator = new ImporterValidator();
    }

    [Fact]
    public void Validate_WithValidEntity_ReturnsValidResult()
    {
        // Arrange
        var entity = new ImporterEntity
        {
            Name = "Test Importer",
            ImporterType = "FileImporter",
            Protocol = "File",
            Version = "1.0.0",
            OutputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "Field1",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            Capabilities = new List<string> { "ReadFile" }
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
        var entity = new ImporterEntity
        {
            ImporterType = "FileImporter",
            Protocol = "File",
            Version = "1.0.0",
            OutputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "Field1",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            Capabilities = new List<string> { "ReadFile" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.Name));
    }

    [Fact]
    public void Validate_WithMissingImporterType_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new ImporterEntity
        {
            Name = "Test Importer",
            Protocol = "File",
            Version = "1.0.0",
            OutputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "Field1",
                        DataType = "String",
                        IsRequired = true
                    }
                }
            },
            Capabilities = new List<string> { "ReadFile" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.ImporterType));
    }

    [Fact]
    public void Validate_WithMissingOutputSchema_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new ImporterEntity
        {
            Name = "Test Importer",
            ImporterType = "FileImporter",
            Protocol = "File",
            Version = "1.0.0",
            Capabilities = new List<string> { "ReadFile" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.OutputSchema));
    }

    [Fact]
    public void Validate_WithEmptyOutputSchemaFields_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new ImporterEntity
        {
            Name = "Test Importer",
            ImporterType = "FileImporter",
            Protocol = "File",
            Version = "1.0.0",
            OutputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>()
            },
            Capabilities = new List<string> { "ReadFile" }
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
        var entity = new ImporterEntity
        {
            Name = "Test Importer",
            ImporterType = "FileImporter",
            Protocol = "File",
            Version = "1.0.0",
            OutputSchema = new SchemaDefinition
            {
                Fields = new List<SchemaField>
                {
                    new SchemaField
                    {
                        Name = "Field1",
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
