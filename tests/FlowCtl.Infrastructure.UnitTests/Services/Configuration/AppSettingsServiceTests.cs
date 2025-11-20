using FlowCtl.Core.Models.Configuration;
using FlowCtl.Core.Services.Location;
using FlowCtl.Infrastructure.Serialization;
using FlowCtl.Infrastructure.Services.Configuration;

namespace FlowCtl.Infrastructure.UnitTests.Services.Configuration;

public class AppSettingsServiceTests : IDisposable
{
    private readonly string _rootPath;
    private readonly TestLocation _location;
    private readonly AppSettingsService _service;

    public AppSettingsServiceTests()
    {
        _rootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_rootPath);
        _location = new TestLocation(_rootPath);
        _service = new AppSettingsService(_location, new JsonSerializer(), new JsonDeserializer());
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnDefaults_WhenFileIsMissing()
    {
        var settings = await _service.LoadAsync();

        Assert.Equal(DeploymentMode.Binary, settings.DeploymentMode);
        Assert.Equal(Path.Combine(_location.DefaultFlowSynxDirectoryName, "data"), settings.Docker.HostDataPath);
        Assert.Equal("flowsynx/flowsynx", settings.Docker.ImageName);
        Assert.Equal(6262, settings.Docker.Port);
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistSettingsToDisk()
    {
        var settings = new AppSettings
        {
            DeploymentMode = DeploymentMode.Docker,
            Docker = new DockerSettings
            {
                ImageName = "flowsynx/flowsynx",
                Tag = "1.2.3-linux-amd64",
                ContainerName = "flowsynx-engine",
                HostDataPath = Path.Combine(_rootPath, "data"),
                ContainerDataPath = "/app/data",
                Port = 7000
            }
        };

        await _service.SaveAsync(settings);

        var reloaded = await _service.LoadAsync();

        Assert.Equal(DeploymentMode.Docker, reloaded.DeploymentMode);
        Assert.Equal("flowsynx-engine", reloaded.Docker.ContainerName);
        Assert.Equal("1.2.3-linux-amd64", reloaded.Docker.Tag);
        Assert.Equal(7000, reloaded.Docker.Port);
        Assert.Equal(Path.Combine(_rootPath, "data"), reloaded.Docker.HostDataPath);
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
