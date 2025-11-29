using System.Runtime.InteropServices;
using FlowCtl.Core.Models.Configuration;
using FlowCtl.Core.Models.Docker;
using FlowCtl.Core.Services.Configuration;
using FlowCtl.Core.Services.Docker;
using FlowCtl.Core.Services.Extractor;
using FlowCtl.Core.Services.Github;
using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Init;

internal class InitCommandOptionsHandler : ICommandOptionsHandler<InitCommandOptions>
{
    private const int EngineContainerPort = 6262;
    private const string DefaultContainerName = "flowsynx-engine";
    private const string DefaultContainerDataPath = "/app/data";
    private const string DefaultImageName = "flowsynx/flowsynx";

    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IGitHubReleaseManager _gitHubReleaseManager;
    private readonly ILocation _location;
    private readonly IArchiveExtractor _archiveExtractor;
    private readonly IDockerService _dockerService;
    private readonly IAppSettingsService _appSettingsService;

    public InitCommandOptionsHandler(
        IFlowCtlLogger flowCtlLogger,
        IGitHubReleaseManager gitHubReleaseManager,
        ILocation location,
        IArchiveExtractor archiveExtractor,
        IDockerService dockerService,
        IAppSettingsService appSettingsService)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _gitHubReleaseManager = gitHubReleaseManager ?? throw new ArgumentNullException(nameof(gitHubReleaseManager));
        _location = location ?? throw new ArgumentNullException(nameof(location));
        _archiveExtractor = archiveExtractor ?? throw new ArgumentNullException(nameof(archiveExtractor));
        _dockerService = dockerService ?? throw new ArgumentNullException(nameof(dockerService));
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
    }

    public async Task<int> HandleAsync(InitCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(InitCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _flowCtlLogger.Write(Resources.Commands_Init_BeginningInitialize);

            if (options.Docker)
            {
                await InitializeDockerAsync(options, cancellationToken);
                return;
            }

            await InitializeBinaryAsync(options, cancellationToken);
        }
        catch (Exception e)
        {
            _flowCtlLogger.WriteError(e.Message);
        }
    }

    private async Task InitializeBinaryAsync(InitCommandOptions options, CancellationToken cancellationToken)
    {
        var flowSynxPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "engine");
        var flowSynxBinaryFile = _location.LookupFlowSynxBinaryFilePath(flowSynxPath);

        if (File.Exists(flowSynxBinaryFile))
        {
            _flowCtlLogger.Write(Resources.Commands_Init_FlowSynxSystemIsAlreadyInitialized);
            return;
        }

        Directory.CreateDirectory(flowSynxPath);

        var initFlowSynx = await InitFlowSynx(options.FlowSynxVersion, cancellationToken);
        if (!initFlowSynx)
            return;

        _flowCtlLogger.Write(Resources.Commands_Init_StartChangeExecutionMode);
        MakeExecutable(flowSynxBinaryFile);

        var consolePath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "console");
        var consoleBinaryFile = _location.LookupConsoleBinaryFilePath(consolePath);
        if (File.Exists(consoleBinaryFile))
        {
            _flowCtlLogger.Write(Resources.Commands_Init_FlowSynxSystemIsAlreadyInitialized);
            return;
        }

        var initConsole = await InitConsole(options.ConsoleVersion, cancellationToken);
        if (!initConsole)
            return;

        _flowCtlLogger.Write(Resources.Commands_Init_StartChangeExecutionMode);
        MakeExecutable(consoleBinaryFile);

        await SaveBinaryModeAsync(cancellationToken);

        _flowCtlLogger.Write(string.Format(Resources.Commands_Init_FlowSynxSystemDownloadedAndInstalledSuccessfully,
            _location.DefaultFlowSynxBinaryDirectoryName));
    }

    private async Task InitializeDockerAsync(InitCommandOptions options, CancellationToken cancellationToken)
    {
        var appSettings = await _appSettingsService.LoadAsync(cancellationToken);
        appSettings.Docker ??= new DockerSettings();

        var dockerAvailable = await _dockerService.IsDockerAvailableAsync(cancellationToken);
        if (!dockerAvailable)
        {
            _flowCtlLogger.WriteError(Resources.Commands_Init_DockerUnavailable);
            _flowCtlLogger.Write(Resources.Commands_Init_UseBinaryFallback);
            return;
        }

        var containerName = string.IsNullOrWhiteSpace(options.ContainerName)
            ? DefaultContainerName
            : options.ContainerName!;
        var port = options.Port > 0 ? options.Port : EngineContainerPort;
        var imageName = string.IsNullOrWhiteSpace(appSettings.Docker.ImageName)
            ? DefaultImageName
            : appSettings.Docker.ImageName;

        var baseVersion = await ResolveFlowSynxImageVersion(options.FlowSynxVersion, cancellationToken);
        if (string.IsNullOrWhiteSpace(baseVersion))
        {
            _flowCtlLogger.WriteError(Resources.Commands_Init_FailedToResolveVersion);
            return;
        }

        var platformSuffix = await GetPlatformSuffix(cancellationToken);
        if (string.IsNullOrWhiteSpace(platformSuffix))
        {
            _flowCtlLogger.WriteError(Resources.Commands_Init_UnsupportedPlatform);
            return;
        }

        var tag = $"{baseVersion}-{platformSuffix}";
        var (hostDataPath, containerDataPath) = ResolveMountPaths(options, appSettings);

        _flowCtlLogger.Write(string.Format(Resources.Commands_Init_PullingDockerImage, $"{imageName}:{tag}"));
        var pullResult = await _dockerService.PullImageAsync(imageName, tag, cancellationToken);
        if (!pullResult.Success)
        {
            _flowCtlLogger.WriteError(string.IsNullOrWhiteSpace(pullResult.Error)
                ? Resources.Commands_Init_DockerPullFailed
                : pullResult.Error);
            return;
        }

        if (await _dockerService.ContainerExistsAsync(containerName, cancellationToken))
        {
            _flowCtlLogger.Write(string.Format(Resources.Commands_Init_RemovingExistingContainer, containerName));
            await _dockerService.RemoveContainerAsync(containerName, force: true, cancellationToken);
        }

        var runOptions = new DockerRunOptions
        {
            ImageName = imageName,
            Tag = tag,
            ContainerName = containerName,
            HostPort = port,
            ContainerPort = EngineContainerPort,
            HostDataPath = hostDataPath,
            ContainerDataPath = containerDataPath,
            Detached = true,
            AdditionalArguments = " --start"
        };

        _flowCtlLogger.Write(string.Format(Resources.Commands_Init_CreatingDockerContainer, containerName, port));
        var runResult = await _dockerService.RunContainerAsync(runOptions, cancellationToken);
        if (!runResult.Success)
        {
            _flowCtlLogger.WriteError(string.IsNullOrWhiteSpace(runResult.Error)
                ? Resources.Commands_Init_DockerRunFailed
                : runResult.Error);
            return;
        }

        appSettings.DeploymentMode = DeploymentMode.Docker;
        appSettings.Docker = new DockerSettings
        {
            ImageName = imageName,
            Tag = tag,
            ContainerName = containerName,
            Port = port,
            HostDataPath = hostDataPath,
            ContainerDataPath = containerDataPath
        };

        await _appSettingsService.SaveAsync(appSettings, cancellationToken);
        _flowCtlLogger.Write(string.Format(Resources.Commands_Init_DockerSuccess, containerName, port, hostDataPath));
    }

    private async Task<bool> InitFlowSynx(string? version, CancellationToken cancellationToken)
    {
        var flowSynxVersion = await _gitHubReleaseManager.GetLatestVersion(GitHubSettings.Organization, GitHubSettings.FlowSynxRepository);
        if (!string.IsNullOrEmpty(version))
            flowSynxVersion = version;

        flowSynxVersion = NormalizeVersion(flowSynxVersion);

        _flowCtlLogger.Write(Resources.Commands_Init_StartDownloadFlowSynxSystemBinary);
        string tempPath = Path.Combine(Path.GetTempPath(), GitHubSettings.FlowSynxArchiveTemporaryFileName);
        var flowSynxDownloadPath = await _gitHubReleaseManager.DownloadAsset(
            GitHubSettings.Organization,
            GitHubSettings.FlowSynxRepository,
            GitHubSettings.FlowSynxArchiveFileName,
            tempPath,
            flowSynxVersion);

        string tempHashPath = Path.Combine(Path.GetTempPath(), GitHubSettings.FlowSynxArchiveTemporaryHashFileName);
        var flowSynxHashDownloadPath = await _gitHubReleaseManager.DownloadAsset(
            GitHubSettings.Organization,
            GitHubSettings.FlowSynxRepository,
            GitHubSettings.FlowSynxArchiveHashFileName,
            tempHashPath,
            flowSynxVersion);

        _flowCtlLogger.Write(Resources.Commands_Init_StartValidatingFlowSynxSystemBinary);
        var isFlowSynxValid = _gitHubReleaseManager.ValidateDownloadedAsset(flowSynxDownloadPath, flowSynxHashDownloadPath);

        if (!isFlowSynxValid)
        {
            _flowCtlLogger.Write(Resources.Commands_Init_ValidatingFlowSynxSystemFailed);
            _flowCtlLogger.Write(Resources.Commands_Init_DownloadedDataCorrupted);
            return false;
        }

        _flowCtlLogger.Write(Resources.Commands_Init_StartExtractingFlowSynxSystemBinary);
        ExtractAsset(flowSynxDownloadPath, "engine", cancellationToken);
        return true;
    }

    private async Task<bool> InitConsole(string? version, CancellationToken cancellationToken)
    {
        var consoleVersion = await _gitHubReleaseManager.GetLatestVersion(GitHubSettings.Organization, GitHubSettings.ConsoleRepository);
        if (!string.IsNullOrEmpty(version))
            consoleVersion = version;

        consoleVersion = NormalizeVersion(consoleVersion);

        _flowCtlLogger.Write(Resources.Commands_Init_StartDownloadConsoleBinary);
        string tempPath = Path.Combine(Path.GetTempPath(), GitHubSettings.ConsoleArchiveTemporaryFileName);
        var consoleDownloadPath = await _gitHubReleaseManager.DownloadAsset(
            GitHubSettings.Organization,
            GitHubSettings.ConsoleRepository,
            GitHubSettings.ConsoleArchiveFileName,
            tempPath,
            consoleVersion);

        string tempHashPath = Path.Combine(Path.GetTempPath(), GitHubSettings.ConsoleArchiveTemporaryHashFileName);
        var consoleHashDownloadPath = await _gitHubReleaseManager.DownloadAsset(
            GitHubSettings.Organization,
            GitHubSettings.ConsoleRepository,
            GitHubSettings.ConsoleArchiveHashFileName,
            tempHashPath,
            consoleVersion);

        _flowCtlLogger.Write(Resources.Commands_Init_StartValidatingConsoleBinary);
        var isConsoleValid = _gitHubReleaseManager.ValidateDownloadedAsset(consoleDownloadPath, consoleHashDownloadPath);

        if (!isConsoleValid)
        {
            _flowCtlLogger.Write(Resources.Commands_Init_ValidatingConsoleFailed);
            return false;
        }

        _flowCtlLogger.Write(Resources.Commands_Init_StartExtractingConsoleBinary);
        ExtractAsset(consoleDownloadPath, "console", cancellationToken);
        return true;
    }

    private void ExtractAsset(string sourcePath, string destinationPathName, CancellationToken cancellationToken)
    {
        var extractTarget = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, destinationPathName, "downloadedFiles");
        var destinationPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, destinationPathName);

        Directory.CreateDirectory(extractTarget);
        _archiveExtractor.ExtractArchive(sourcePath, extractTarget);
        Directory.CreateDirectory(destinationPath);
        CopyFilesRecursively(extractTarget, destinationPath, cancellationToken);
        Directory.Delete(extractTarget, true);
    }

    private string NormalizeVersion(string? version)
    {
        if (string.IsNullOrEmpty(version))
            return string.Empty;

        return !version.StartsWith("v") ? $"v{version}" : version;
    }

    private static string NormalizeDockerVersion(string? version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return string.Empty;

        return version.StartsWith("v", StringComparison.OrdinalIgnoreCase)
            ? version.Substring(1)
            : version;
    }

    private void CopyFilesRecursively(string sourcePath, string targetPath, CancellationToken cancellationToken)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }

    private void MakeExecutable(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        const UnixFileMode ownershipPermissions = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                                                  UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute |
                                                  UnixFileMode.OtherRead | UnixFileMode.OtherWrite | UnixFileMode.OtherExecute;

        File.SetUnixFileMode(path, ownershipPermissions);
    }

    private async Task SaveBinaryModeAsync(CancellationToken cancellationToken)
    {
        var appSettings = await _appSettingsService.LoadAsync(cancellationToken);
        appSettings.DeploymentMode = DeploymentMode.Binary;
        await _appSettingsService.SaveAsync(appSettings, cancellationToken);
    }

    private async Task<string> ResolveFlowSynxImageVersion(string? requestedVersion, CancellationToken cancellationToken)
    {
        var version = await _gitHubReleaseManager.GetLatestVersion(GitHubSettings.Organization, GitHubSettings.FlowSynxRepository);
        if (!string.IsNullOrWhiteSpace(requestedVersion))
            version = requestedVersion;

        return NormalizeDockerVersion(version);
    }

    private async Task<string?> GetPlatformSuffix(CancellationToken cancellationToken)
    {
        var mode = await _dockerService.GetDockerModeAsync(cancellationToken);
        if (mode == "Windows")
            return "windows-ltsc2022-amd64";

        return "linux-amd64";
    }

    private (string hostDataPath, string containerDataPath) ResolveMountPaths(InitCommandOptions options, AppSettings appSettings)
    {
        var hostPath = !string.IsNullOrWhiteSpace(options.Mount)
            ? options.Mount!
            : appSettings.Docker.HostDataPath;

        if (string.IsNullOrWhiteSpace(hostPath))
            hostPath = Path.Combine(_location.DefaultFlowSynxDirectoryName, "data");

        var containerPath = !string.IsNullOrWhiteSpace(options.ContainerPath)
            ? options.ContainerPath!
            : appSettings.Docker.ContainerDataPath;

        if (string.IsNullOrWhiteSpace(containerPath))
            containerPath = DefaultContainerDataPath;

        return (hostPath, containerPath);
    }
}
