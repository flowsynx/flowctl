﻿using FlowCtl.Core.Exceptions;
using FlowCtl.Core.Serialization;
using FlowCtl.Infrastructure.Serialization;

namespace FlowCtl.Infrastructure.UnitTests.Serialization;

public class JsonSerializerTests
{
    [Fact]
    public void ContentMineType_ShouldExposeJsonMimeTypeWithoutInstance()
    {
        // Act
        var mimeType = JsonSerializer.ContentMineType;

        // Assert
        Assert.Equal("application/json", mimeType);
    }

    [Fact]
    public void Serialize_ShouldThrowFlowSynxException_WhenInputIsNull()
    {
        // Arrange
        var jsonSerializer = new JsonSerializer();
        object? input = null;

        // Act & Assert
        var exception = Assert.Throws<FlowCtlException>(() => jsonSerializer.Serialize(input));

        // Assert
        Assert.Equal("Input value can't be empty or null.", exception.Message);
    }

    [Fact]
    public void Serialize_ShouldReturnsSerializedString_WhenValidInput()
    {
        // Arrange
        var jsonSerializer = new JsonSerializer();
        var input = new Person { Name = "Amin Ziagham", Age = 30 };

        // Act
        var result = jsonSerializer.Serialize(input);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("\"Name\":\"Amin Ziagham\"", result);
        Assert.Contains("\"Age\":30", result);
    }

    [Fact]
    public void Serialize_ShouldReturnsIndentedJson_WhenWithIndentedConfiguration()
    {
        // Arrange
        var jsonSerializer = new JsonSerializer();
        var input = new Person { Name = "Amin Ziagham", Age = 30 };
        var configuration = new JsonSerializationConfiguration { Indented = true };

        // Act
        var result = jsonSerializer.Serialize(input, configuration);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("\"Name\": \"Amin Ziagham\"", result); // Should be indented
        Assert.Contains("\"Age\": 30", result);
        Assert.Contains("\n", result); // Indicates indentation
    }

    [Fact]
    public void Serialize_ShouldHandleSerializationException_WhenJsonIsInvalid()
    {
        // Arrange
        var jsonSerializer = new JsonSerializer();
        var invalidInput = new UnserializableClass { SomeAction = () => Console.WriteLine("Hello") };

        // Act & Assert
        Assert.Throws<FlowCtlException>(() => jsonSerializer.Serialize(invalidInput));
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class UnserializableClass
    {
        public Action? SomeAction { get; set; } // Action cannot be serialized
    }
}
