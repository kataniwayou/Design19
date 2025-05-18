using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using Xunit;

namespace FlowOrchestrator.Domain.Tests.Validation;

public class FlowEntityValidatorTests
{
    private readonly FlowEntityValidator _validator;

    public FlowEntityValidatorTests()
    {
        _validator = new FlowEntityValidator();
    }

    [Fact]
    public void Validate_WithValidEntity_ReturnsValidResult()
    {
        // Arrange
        var entity = new FlowEntity
        {
            Name = "Test Flow",
            ImporterId = "importer-1",
            ProcessingChainIds = new List<string> { "chain-1" },
            ExporterIds = new List<string> { "exporter-1" },
            BranchConfigurations = new Dictionary<string, Abstractions.Interfaces.FlowBranchConfiguration>
            {
                {
                    "branch-1",
                    new Abstractions.Interfaces.FlowBranchConfiguration
                    {
                        BranchId = "branch-1",
                        ProcessingChainId = "chain-1",
                        ExporterId = "exporter-1"
                    }
                }
            }
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
        var entity = new FlowEntity
        {
            Name = "",
            ImporterId = "importer-1",
            ProcessingChainIds = new List<string> { "chain-1" },
            ExporterIds = new List<string> { "exporter-1" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.Name));
    }

    [Fact]
    public void Validate_WithMissingImporterId_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new FlowEntity
        {
            Name = "Test Flow",
            ImporterId = "",
            ProcessingChainIds = new List<string> { "chain-1" },
            ExporterIds = new List<string> { "exporter-1" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.ImporterId));
    }

    [Fact]
    public void Validate_WithEmptyProcessingChainIds_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new FlowEntity
        {
            Name = "Test Flow",
            ImporterId = "importer-1",
            ProcessingChainIds = new List<string>(),
            ExporterIds = new List<string> { "exporter-1" }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.ProcessingChainIds));
    }

    [Fact]
    public void Validate_WithEmptyExporterIds_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new FlowEntity
        {
            Name = "Test Flow",
            ImporterId = "importer-1",
            ProcessingChainIds = new List<string> { "chain-1" },
            ExporterIds = new List<string>()
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(entity.ExporterIds));
    }

    [Fact]
    public void Validate_WithInvalidBranchConfiguration_ReturnsInvalidResult()
    {
        // Arrange
        var entity = new FlowEntity
        {
            Name = "Test Flow",
            ImporterId = "importer-1",
            ProcessingChainIds = new List<string> { "chain-1" },
            ExporterIds = new List<string> { "exporter-1" },
            BranchConfigurations = new Dictionary<string, Abstractions.Interfaces.FlowBranchConfiguration>
            {
                {
                    "branch-1",
                    new Abstractions.Interfaces.FlowBranchConfiguration
                    {
                        BranchId = "branch-1",
                        ProcessingChainId = "invalid-chain",
                        ExporterId = "exporter-1"
                    }
                }
            }
        };

        // Act
        var result = _validator.Validate(entity);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("ProcessingChainId"));
    }
}
