using FlowOrchestrator.IntegrationBase.Messaging;
using Xunit;

namespace FlowOrchestrator.IntegrationBase.Tests.Messaging;

public class SchemaDefinitionTests
{
    [Fact]
    public void SchemaDefinition_DefaultProperties_ShouldBeInitialized()
    {
        // Arrange & Act
        var schema = new SchemaDefinition();

        // Assert
        Assert.NotNull(schema.Name);
        Assert.NotNull(schema.Version);
        Assert.NotNull(schema.Format);
        Assert.NotNull(schema.Definition);
        Assert.NotNull(schema.Fields);
        Assert.Empty(schema.Fields);
    }

    [Fact]
    public void SchemaDefinition_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var schema = new SchemaDefinition
        {
            Name = "TestSchema",
            Version = "1.0.0",
            Format = "JSON",
            Definition = "{\"type\": \"object\"}",
            Fields = new List<SchemaField>
            {
                new SchemaField
                {
                    Name = "TestField",
                    Type = "string",
                    Required = true,
                    Description = "Test field description",
                    DefaultValue = "default",
                    Constraints = new Dictionary<string, object>
                    {
                        { "minLength", 1 },
                        { "maxLength", 100 }
                    }
                }
            }
        };

        // Act & Assert
        Assert.Equal("TestSchema", schema.Name);
        Assert.Equal("1.0.0", schema.Version);
        Assert.Equal("JSON", schema.Format);
        Assert.Equal("{\"type\": \"object\"}", schema.Definition);
        Assert.Single(schema.Fields);
        
        var field = schema.Fields[0];
        Assert.Equal("TestField", field.Name);
        Assert.Equal("string", field.Type);
        Assert.True(field.Required);
        Assert.Equal("Test field description", field.Description);
        Assert.Equal("default", field.DefaultValue);
        Assert.Equal(2, field.Constraints.Count);
        Assert.Equal(1, field.Constraints["minLength"]);
        Assert.Equal(100, field.Constraints["maxLength"]);
    }

    [Fact]
    public void SchemaField_DefaultProperties_ShouldBeInitialized()
    {
        // Arrange & Act
        var field = new SchemaField();

        // Assert
        Assert.NotNull(field.Name);
        Assert.NotNull(field.Type);
        Assert.NotNull(field.Description);
        Assert.NotNull(field.Constraints);
        Assert.Empty(field.Constraints);
        Assert.False(field.Required);
        Assert.Null(field.DefaultValue);
    }
}
