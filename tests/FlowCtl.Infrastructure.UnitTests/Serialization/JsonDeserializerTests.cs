using FlowCtl.Core.Exceptions;
using FlowCtl.Core.Serialization;
using FlowCtl.Infrastructure.Serialization;

namespace FlowCtl.Infrastructure.UnitTests.Serialization;

public class JsonDeserializerTests
{
    private readonly JsonDeserializer _jsonDeserializer;

    public JsonDeserializerTests()
    {
        _jsonDeserializer = new JsonDeserializer();
    }

    [Fact]
    public void Deserialize_ShouldThrowFlowSynxException_WhenInputIsNull()
    {
        // Arrange
        string? input = null;

        // Act
        var exception = Assert.Throws<FlowCtlException>(() => _jsonDeserializer.Deserialize<object>(input));

        // Assert
        Assert.Equal("Input value can't be empty or null.", exception.Message);
    }

    [Fact]
    public void Deserialize_ShouldThrowFlowSynxException_WhenInputIsEmpty()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var exception = Assert.Throws<FlowCtlException>(() => _jsonDeserializer.Deserialize<object>(input));

        // Assert
        Assert.Equal("Input value can't be empty or null.", exception.Message);
    }

    [Fact]
    public void Deserialize_ShouldDeserializeObject_WhenValidJson()
    {
        // Arrange
        string input = "{\"name\":\"Amin Ziagham\",\"age\":30}";

        // Act
        var result = _jsonDeserializer.Deserialize<Person>(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Amin Ziagham", result.Name);
        Assert.Equal(30, result.Age);
    }

    [Fact]
    public void Deserialize_ShouldApplyIndentedFormatting_WhenConfigurationIsSetToIndented()
    {
        // Arrange
        string input = "{\"name\":\"Amin Ziagham\",\"age\":30}";
        var config = new JsonSerializationConfiguration { Indented = true };

        // Act
        var result = _jsonDeserializer.Deserialize<Person>(input, config);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Amin Ziagham", result.Name);
        Assert.Equal(30, result.Age);
    }

    [Fact]
    public void Deserialize_ShouldHandleSerializationException_WhenJsonIsInvalid()
    {
        // Arrange
        string invalidJson = "{\"name\":\"Amin Ziagham\", \"age\":}";

        // Act
        var exception = Assert.Throws<FlowCtlException>(() => _jsonDeserializer.Deserialize<Person>(invalidJson));

        // Assert
        Assert.Contains("Unexpected character encountered while parsing value: }. Path 'age'", exception.Message);
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}