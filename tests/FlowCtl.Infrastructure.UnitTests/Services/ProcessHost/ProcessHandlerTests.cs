using FlowCtl.Core.Services.Logger;
using FlowCtl.Infrastructure.Services.ProcessHost;
using Moq;

namespace FlowCtl.Infrastructure.UnitTests.Services.ProcessHost;

public class ProcessHandlerTests
{
    private readonly Mock<IFlowCtlLogger> _loggerMock;
    private readonly Mock<IProcessProvider> _processProviderMock;
    private readonly ProcessHandler _handler;

    public ProcessHandlerTests()
    {
        _loggerMock = new Mock<IFlowCtlLogger>();
        _processProviderMock = new Mock<IProcessProvider>();
        _handler = new ProcessHandler(_loggerMock.Object, _processProviderMock.Object);
    }

    [Fact]
    public void IsRunning_ReturnsTrue_WhenProcessExists()
    {
        var processMock = new Mock<IProcessWrapper>();
        _processProviderMock.Setup(p => p.GetProcessesByName("myproc", "localhost"))
            .Returns(new[] { processMock.Object });

        var result = _handler.IsRunning("myproc", "localhost");

        Assert.True(result);
    }

    [Fact]
    public void IsRunning_ReturnsFalse_WhenNoProcesses()
    {
        _processProviderMock.Setup(p => p.GetProcessesByName("myproc", "localhost"))
            .Returns(Array.Empty<IProcessWrapper>());

        var result = _handler.IsRunning("myproc", "localhost");

        Assert.False(result);
    }

    [Fact]
    public void Terminate_KillsProcesses_AndLogs()
    {
        var processMock = new Mock<IProcessWrapper>();
        processMock.Setup(p => p.ProcessName).Returns("myproc");

        _processProviderMock.Setup(p => p.GetProcessesByName("myproc", "localhost"))
            .Returns(new[] { processMock.Object });

        _handler.Terminate("myproc", "localhost");

        processMock.Verify(p => p.Kill(), Times.Once);
        _loggerMock.Verify(l => l.Write("Process 'myproc' killed."), Times.Once);
    }

    [Fact]
    public void Terminate_DoesNothing_WhenNoProcesses()
    {
        _processProviderMock.Setup(p => p.GetProcessesByName("myproc", "localhost"))
            .Returns(Array.Empty<IProcessWrapper>());

        _handler.Terminate("myproc", "localhost");

        _loggerMock.Verify(l => l.Write(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void IsStopped_ReturnsTrue_WhenNotRunning()
    {
        _processProviderMock.Setup(p => p.GetProcessesByName("myproc", "localhost"))
            .Returns(Array.Empty<IProcessWrapper>());

        var result = _handler.IsStopped("myproc", "localhost", force: false);

        Assert.True(result);
    }

    [Fact]
    public void IsStopped_ReturnsFalse_WhenRunning_AndNotForce()
    {
        var processMock = new Mock<IProcessWrapper>();
        _processProviderMock.Setup(p => p.GetProcessesByName("myproc", "localhost"))
            .Returns(new[] { processMock.Object });

        var result = _handler.IsStopped("myproc", "localhost", force: false);

        Assert.False(result);
    }

    [Fact]
    public void IsStopped_KillsProcesses_AndReturnsTrue_WhenForceIsTrue()
    {
        var processMock = new Mock<IProcessWrapper>();
        processMock.Setup(p => p.ProcessName).Returns("myproc");

        _processProviderMock.Setup(p => p.GetProcessesByName("myproc", "localhost"))
            .Returns(new[] { processMock.Object });

        var result = _handler.IsStopped("myproc", "localhost", force: true);

        Assert.True(result);
        processMock.Verify(p => p.Kill(), Times.Once);
        _loggerMock.Verify(l => l.Write("Process 'myproc' killed."), Times.Once);
    }
}