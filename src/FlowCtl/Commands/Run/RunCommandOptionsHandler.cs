using System.Diagnostics;
using System.Runtime.InteropServices;
using FlowCtl.Core.Models.Configuration;
using FlowCtl.Core.Models.Docker;
using FlowCtl.Core.Services.Configuration;
using FlowCtl.Core.Services.Docker;
using FlowCtl.Core.Services.Github;
using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Run;

internal class RunCommandOptionsHandler : ICommandOptionsHandler<RunCommandOptions>
{
    private const int EngineContainerPort = 6262;
    private const string DefaultContainerName = "flowsynx-engine";
    private const string DefaultContainerDataPath = "/app/data";
    private const string DefaultImageName = "flowsynx/flowsynx";

    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;
    private readonly IDockerService _dockerService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly IGitHubReleaseManager _gitHubReleaseManager;

    public RunCommandOptionsHandler(
        IFlowCtlLogger flowCtlLogger,
        ILocation location,
        IDockerService dockerService,
        IAppSettingsService appSettingsService,
        IGitHubReleaseManager gitHubReleaseManager)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _location = location ?? throw new ArgumentNullException(nameof(location));
        _dockerService = dockerService ?? throw new ArgumentNullException(nameof(dockerService));
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
        _gitHubReleaseManager = gitHubReleaseManager ?? throw new ArgumentNullException(nameof(gitHubReleaseManager));
    }

    public async Task<int> HandleAsync(RunCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(RunCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (options.Docker)
            {
                await RunDockerAsync(options, cancellationToken);
                return;
            }

            var appSettings = await _appSettingsService.LoadAsync(cancellationToken);
            var flowSynxPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "engine");
            var flowSynxBinaryFile = _location.LookupFlowSynxBinaryFilePath(flowSynxPath);

            if (appSettings.DeploymentMode == DeploymentMode.Docker && !Path.Exists(flowSynxBinaryFile))
            {
                _flowCtlLogger.Write(Resources.Command_Run_DockerModeHint);
                return;
            }

            await RunBinaryAsync(options, flowSynxPath, flowSynxBinaryFile);
        }
        catch (Exception e)
        {
            _flowCtlLogger.WriteError(e.Message);
        }
    }

    private Task RunBinaryAsync(RunCommandOptions options, string flowSynxPath, string flowSynxBinaryFile)
    {
        if (!Path.Exists(flowSynxBinaryFile))
        {
            _flowCtlLogger.WriteError(Resources.Commands_Run_FlowSynxIsNotInstalled);
            return Task.CompletedTask;
        }

        var startInfo = new ProcessStartInfo(flowSynxBinaryFile)
        {
            Arguments = GetArgumentStr(options),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = flowSynxPath
        };

        var process = new Process { StartInfo = startInfo };

        if (!options.Background)
        {
            process.OutputDataReceived += OutputDataHandler;
            process.ErrorDataReceived += ErrorDataHandler;
        }

        process.Start();

        if (!options.Background)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }

        return Task.CompletedTask;
    }

    private async Task RunDockerAsync(RunCommandOptions options, CancellationToken cancellationToken)
    {
        var dockerAvailable = await _dockerService.IsDockerAvailableAsync(cancellationToken);
        if (!dockerAvailable)
        {
            _flowCtlLogger.WriteError(Resources.Commands_Init_DockerUnavailable);
            return;
        }

        var appSettings = await _appSettingsService.LoadAsync(cancellationToken);
        appSettings.Docker ??= new DockerSettings();
        var dockerSettings = appSettings.Docker;
        var containerName = string.IsNullOrWhiteSpace(dockerSettings.ContainerName)
            ? DefaultContainerName
            : dockerSettings.ContainerName;
        var hostDataPath = string.IsNullOrWhiteSpace(dockerSettings.HostDataPath)
            ? Path.Combine(_location.DefaultFlowSynxDirectoryName, "data")
            : dockerSettings.HostDataPath;
        var containerDataPath = string.IsNullOrWhiteSpace(dockerSettings.ContainerDataPath)
            ? DefaultContainerDataPath
            : dockerSettings.ContainerDataPath;
        var port = dockerSettings.Port > 0 ? dockerSettings.Port : EngineContainerPort;
        var imageName = string.IsNullOrWhiteSpace(dockerSettings.ImageName)
            ? DefaultImageName
            : dockerSettings.ImageName;

        var platformSuffix = GetPlatformSuffix();
        if (string.IsNullOrWhiteSpace(platformSuffix))
        {
            _flowCtlLogger.WriteError(Resources.Commands_Init_UnsupportedPlatform);
            return;
        }

        var baseVersion = await ResolveFlowSynxImageVersion(cancellationToken);
        if (string.IsNullOrWhiteSpace(baseVersion) && string.IsNullOrWhiteSpace(dockerSettings.Tag))
        {
            _flowCtlLogger.WriteError(Resources.Command_Run_FailedToResolveVersion);
            return;
        }

        var tag = string.IsNullOrWhiteSpace(dockerSettings.Tag)
            ? $"{baseVersion}-{platformSuffix}"
            : dockerSettings.Tag;

        Directory.CreateDirectory(hostDataPath);

        if (!await _dockerService.ContainerExistsAsync(containerName, cancellationToken))
        {
            _flowCtlLogger.Write(string.Format(Resources.Command_Run_DockerPull, $"{imageName}:{tag}"));
            var pullResult = await _dockerService.PullImageAsync(imageName, tag, cancellationToken);
            if (!pullResult.Success)
            {
                _flowCtlLogger.WriteError(string.IsNullOrWhiteSpace(pullResult.Error)
                    ? Resources.Command_Run_DockerPullFailed
                    : pullResult.Error);
                return;
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
                Detached = true
            };

            _flowCtlLogger.Write(string.Format(Resources.Command_Run_DockerCreatingContainer, containerName, port));
            var runResult = await _dockerService.RunContainerAsync(runOptions, cancellationToken);
            if (!runResult.Success)
            {
                _flowCtlLogger.WriteError(string.IsNullOrWhiteSpace(runResult.Error)
                    ? Resources.Command_Run_DockerRunFailed
                    : runResult.Error);
                return;
            }
        }
        else
        {
            var containerRunning = await _dockerService.IsContainerRunningAsync(containerName, cancellationToken);
            if (!containerRunning)
            {
                _flowCtlLogger.Write(string.Format(Resources.Command_Run_StartingDockerContainer, containerName));
                var startResult = await _dockerService.StartContainerAsync(containerName, cancellationToken);
                if (!startResult.Success)
                {
                    _flowCtlLogger.WriteError(string.IsNullOrWhiteSpace(startResult.Error)
                        ? Resources.Command_Run_DockerRunFailed
                        : startResult.Error);
                    return;
                }
            }
        }

        appSettings.DeploymentMode = DeploymentMode.Docker;
        appSettings.Docker.ImageName = imageName;
        appSettings.Docker.ContainerName = containerName;
        appSettings.Docker.ContainerDataPath = containerDataPath;
        appSettings.Docker.HostDataPath = hostDataPath;
        appSettings.Docker.Port = port;
        appSettings.Docker.Tag = tag;
        await _appSettingsService.SaveAsync(appSettings, cancellationToken);

        _flowCtlLogger.Write(string.Format(Resources.Command_Run_DockerStarted, containerName, port));

        if (!options.Background)
        {
            _flowCtlLogger.Write(Resources.Command_Run_AttachingToLogs);
            await _dockerService.TailLogsAsync(containerName, cancellationToken);
        }
    }

    private string GetArgumentStr(RunCommandOptions options)
    {
        var argList = new List<string>
        {
            "--start"
        };

        return argList.Count == 0 ? string.Empty : string.Join(' ', argList);
    }

    private void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (outLine.Data != null) _flowCtlLogger.Write(outLine.Data);
    }

    private void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (outLine.Data != null) _flowCtlLogger.WriteError(outLine.Data);
    }

    private async Task<string> ResolveFlowSynxImageVersion(CancellationToken cancellationToken)
    {
        var version = await _gitHubReleaseManager.GetLatestVersion(GitHubSettings.Organization, GitHubSettings.FlowSynxRepository);
        return NormalizeDockerVersion(version);
    }

    private string? GetPlatformSuffix()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "windows-ltsc2022-amd64";

        if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
            return null;

        return "linux-amd64";
    }

    private static string NormalizeDockerVersion(string? version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return string.Empty;

        return version.StartsWith("v", StringComparison.OrdinalIgnoreCase)
            ? version.Substring(1)
            : version;
    }
}
