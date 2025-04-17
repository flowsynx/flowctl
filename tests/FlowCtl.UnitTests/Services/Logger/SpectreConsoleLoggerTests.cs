using FlowCtl.Core.Serialization;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Services.Logger;
using Moq;
using Spectre.Console;
using System.Dynamic;

namespace FlowCtl.UnitTests.Services.Logger;

public class SpectreConsoleLoggerTests
{
    private readonly Mock<IAnsiConsole> _consoleMock = new();
    private readonly Mock<IJsonSerializer> _serializerMock = new();
    private readonly Mock<IJsonDeserializer> _deserializerMock = new();

    private SpectreConsoleLogger CreateLogger()
    {
        return new SpectreConsoleLogger(_consoleMock.Object, _serializerMock.Object, _deserializerMock.Object);
    }

    [Fact]
    public void Constructor_ShouldThrow_IfAnyDependencyIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SpectreConsoleLogger(null!, _serializerMock.Object, _deserializerMock.Object));
        Assert.Throws<ArgumentNullException>(() => new SpectreConsoleLogger(_consoleMock.Object, null!, _deserializerMock.Object));
        Assert.Throws<ArgumentNullException>(() => new SpectreConsoleLogger(_consoleMock.Object, _serializerMock.Object, null!));
    }

    [Theory]
    [InlineData(OutputType.Json)]
    [InlineData(OutputType.Xml)]
    [InlineData(OutputType.Yaml)]
    [InlineData(OutputType.Table)]
    public void Write_WithDifferentOutputTypes_ShouldCallSerializer(OutputType output)
    {
        // Arrange
        var logger = CreateLogger();
        var input = new { Id = 1, Name = "Test" };
        var serialized = "{ \"Id\": 1, \"Name\": \"Test\" }";

        _serializerMock
            .Setup(s => s.Serialize(It.IsAny<object>(), It.IsAny<JsonSerializationConfiguration>()))
            .Returns(serialized);

        _deserializerMock
            .Setup(d => d.Deserialize<ExpandoObject>(It.IsAny<string>(), It.IsAny<JsonSerializationConfiguration>()))
            .Returns(new ExpandoObject());

        _deserializerMock
            .Setup(d => d.Deserialize<List<ExpandoObject>>(It.IsAny<string>(), It.IsAny<JsonSerializationConfiguration>()))
            .Returns(new List<ExpandoObject>());

        // Act
        logger.Write(input, output);

        // Assert
        _serializerMock.Verify(s => s.Serialize(input, It.IsAny<JsonSerializationConfiguration>()), Times.Once);
    }

    [Fact]
    public void GenerateJson_ShouldReturnSameString()
    {
        // Arrange
        var logger = CreateLogger();
        var input = "{ \"value\": 123 }";

        // Act
        var result = logger.GenerateJson(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void GenerateXml_ShouldReturnEmpty_WhenInputIsNull()
    {
        // Arrange
        var logger = CreateLogger();

        // Act
        var result = logger.GenerateXml(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GenerateYaml_ShouldReturnEmpty_WhenInputIsNull()
    {
        // Arrange
        var logger = CreateLogger();

        // Act
        var result = logger.GenerateYaml(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GenerateYaml_ShouldReturnYaml_WhenValidJsonExpandoArray()
    {
        // Arrange
        var logger = CreateLogger();
        var inputJson = "[{ \"key\": \"value\" }]";

        var expando = new ExpandoObject() as IDictionary<string, object?>;
        expando["key"] = "value";

        _deserializerMock
            .Setup(d => d.Deserialize<List<ExpandoObject>>(It.IsAny<string>(), It.IsAny<JsonSerializationConfiguration>()))
            .Returns(new List<ExpandoObject> { (ExpandoObject)expando });

        // Act
        var result = logger.GenerateYaml(inputJson);

        // Assert
        Assert.Contains("key: value", result);
    }
}