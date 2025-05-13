using FlowCtl.ApplicationBuilders;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine;

namespace FlowCtl.UnitTests.ApplicationBuilders;

public class CliApplicationBuilderTests
{
    [Fact]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange
        RootCommand rootCommand = new RootCommand();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CliApplicationBuilder(null!, rootCommand));
    }

    [Fact]
    public void Constructor_WithNullRootCommand_ThrowsArgumentNullException()
    {
        // Arrange
        var serviceProvider = new Mock<IServiceProvider>().Object;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CliApplicationBuilder(serviceProvider, null!));
    }

    [Fact]
    public async Task RunAsync_WithoutCommands_RunsSuccessfully()
    {
        // Arrange
        var services = new ServiceCollection().BuildServiceProvider();
        var rootCommand = new RootCommand();
        var builder = new CliApplicationBuilder(services, rootCommand);

        // Act
        var result = await builder.RunAsync(new[] { "--help" });

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task RunAsync_WithSingleCommand_InvokesCommand()
    {
        // Arrange
        int wasCalled = 0;

        var testCommand = new Command("test", "Test command")
        {
            Handler = CommandHandler.Create(() => wasCalled++)
        };

        var services = new ServiceCollection()
            .AddSingleton<Command>(testCommand)
            .BuildServiceProvider();

        var rootCommand = new RootCommand();
        var builder = new CliApplicationBuilder(services, rootCommand);

        // Act
        var result = await builder.RunAsync(new[] { "test" });

        // Assert
        Assert.Equal(0, result);
        Assert.Equal(1, wasCalled);
    }

    [Fact]
    public async Task RunAsync_AddsAllCommandsToRootCommand()
    {
        // Arrange
        var cmd1 = new Command("cmd1", "Command 1");
        var cmd2 = new Command("cmd2", "Command 2");

        var services = new ServiceCollection()
            .AddSingleton<Command>(cmd1)
            .AddSingleton<Command>(cmd2)
            .BuildServiceProvider();

        var rootCommand = new RootCommand();
        var builder = new CliApplicationBuilder(services, rootCommand);

        // Act
        await builder.RunAsync(new[] { "--help" });

        // Assert
        Assert.Contains(rootCommand.Children, c => c.Name == "cmd1");
        Assert.Contains(rootCommand.Children, c => c.Name == "cmd2");
    }
}