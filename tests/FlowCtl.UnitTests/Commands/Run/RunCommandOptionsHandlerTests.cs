using FlowCtl;
using FlowCtl.Commands.Run;
using FlowCtl.Core.Models.Configuration;
using FlowCtl.Core.Services.Configuration;
using FlowCtl.Core.Services.Docker;
using FlowCtl.Core.Services.Github;
using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;
using Moq;

namespace FlowCtl.UnitTests.Commands.Run;

public class RunCommandOptionsHandlerTests : IDisposable
{
    private readonly string _rootPath;
    private readonly TestLocation _location;

    public RunCommandOptionsHandlerTests()
    {
        _rootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_rootPath);
        _location = new TestLocation(_rootPath);
    }

    [Fact]
    public async Task HandleAsync_ShouldSuggestDockerFlag_WhenDeploymentModeIsDockerAndBinaryIsMissing()
    {
        var loggerMock = new Mock<IFlowCtlLogger>();
        var dockerService = new Mock<IDockerService>();
        var appSettingsService = new Mock<IAppSettingsService>();

        appSettingsService.Setup(s => s.LoadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppSettings
            {
                DeploymentMode = DeploymentMode.Docker,
                Docker = new DockerSettings { HostDataPath = Path.Combine(_location.DefaultFlowSynxDirectoryName, "data") }
            });

        var handler = new RunCommandOptionsHandler(
            loggerMock.Object,
            _location,
            dockerService.Object,
            appSettingsService.Object,
            Mock.Of<IGitHubReleaseManager>());

        await handler.HandleAsync(new RunCommandOptions(), CancellationToken.None);

        loggerMock.Verify(l => l.Write(Resources.Command_Run_DockerModeHint), Times.Once);
    }

    public void Dispose()
    {
        if (Directory.Exists(_rootPath))
            Directory.Delete(_rootPath, true);
    }

    private class TestLocation : ILocation
    {
        public TestLocation(string rootPath)
        {
            RootLocation = rootPath;
            UserProfilePath = rootPath;
            DefaultFlowSynxDirectoryName = Path.Combine(rootPath, ".flowsynx");
            DefaultFlowSynxBinaryDirectoryName = Path.Combine(DefaultFlowSynxDirectoryName, "bin");
        }

        public string RootLocation { get; }
        public string UserProfilePath { get; }
        public string DefaultFlowSynxDirectoryName { get; }
        public string DefaultFlowSynxBinaryDirectoryName { get; }
        public string FlowSynxBinaryName => "flowsynx";
        public string ConsoleBinaryName => "console";

        public string LookupFlowCtlBinaryFilePath(string path) => Path.Combine(path, "flowctl");
        public string LookupFlowSynxBinaryFilePath(string path) => Path.Combine(path, "flowsynx");
        public string LookupConsoleBinaryFilePath(string path) => Path.Combine(path, "console");
    }
}
