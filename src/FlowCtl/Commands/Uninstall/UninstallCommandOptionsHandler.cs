using FlowCtl.Core.Models.Configuration;
using FlowCtl.Core.Services.Configuration;
using FlowCtl.Core.Services.Docker;
using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Core.Services.ProcessHost;

namespace FlowCtl.Commands.Uninstall;

internal class UninstallCommandOptionsHandler : ICommandOptionsHandler<UninstallCommandOptions>
{
    private const string DefaultContainerName = "flowsynx-engine";

    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;
    private readonly IProcessHandler _processHandler;
    private readonly IDockerService _dockerService;
    private readonly IAppSettingsService _appSettingsService;

    public UninstallCommandOptionsHandler(
        IFlowCtlLogger flowCtlLogger,
        ILocation location,
        IProcessHandler processHandler,
        IDockerService dockerService,
        IAppSettingsService appSettingsService)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _location = location ?? throw new ArgumentNullException(nameof(location));
        _processHandler = processHandler ?? throw new ArgumentNullException(nameof(processHandler));
        _dockerService = dockerService ?? throw new ArgumentNullException(nameof(dockerService));
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
    }

    public async Task<int> HandleAsync(UninstallCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(UninstallCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _flowCtlLogger.Write(Resources.Commands_Uninstall_BeginningUninstalling);

            if (options.Docker)
            {
                await UninstallDockerAsync(options, cancellationToken);
                return;
            }

            await UninstallBinaryAsync(options);
        }
        catch (Exception e)
        {
            _flowCtlLogger.WriteError(e.Message);
        }
    }

    private async Task UninstallDockerAsync(UninstallCommandOptions options, CancellationToken cancellationToken)
    {
        if (!await _dockerService.IsDockerAvailableAsync(cancellationToken))
        {
            _flowCtlLogger.WriteError(Resources.Commands_Init_DockerUnavailable);
            return;
        }

        var settings = await _appSettingsService.LoadAsync(cancellationToken);
        settings.Docker ??= new DockerSettings();
        var containerName = string.IsNullOrWhiteSpace(settings.Docker.ContainerName)
            ? DefaultContainerName
            : settings.Docker.ContainerName;

        if (await _dockerService.ContainerExistsAsync(containerName, cancellationToken))
        {
            _flowCtlLogger.Write(string.Format(Resources.Commands_Uninstall_RemovingDockerContainer, containerName));
            var removeResult = await _dockerService.RemoveContainerAsync(containerName, options.Force, cancellationToken);
            if (!removeResult.Success)
            {
                _flowCtlLogger.WriteError(string.IsNullOrWhiteSpace(removeResult.Error)
                    ? Resources.Commands_Uninstall_RemoveDockerFailed
                    : removeResult.Error);
                return;
            }
        }
        else
        {
            _flowCtlLogger.Write(string.Format(Resources.Commands_Uninstall_DockerContainerMissing, containerName));
        }

        if (options.RemoveData)
            RemoveDataDirectory(settings.Docker.HostDataPath);

        settings.DeploymentMode = DeploymentMode.Binary;
        settings.Docker.Tag = string.Empty;
        await _appSettingsService.SaveAsync(settings, cancellationToken);

        _flowCtlLogger.Write(Resources.Commands_Uninstall_UninstalledSuccessfully);
    }

    private Task UninstallBinaryAsync(UninstallCommandOptions options)
    {
        if (_processHandler.IsRunning(_location.FlowSynxBinaryName, "."))
        {
            if (options.Force)
            {
                _processHandler.Terminate(_location.FlowSynxBinaryName, ".");
                _flowCtlLogger.Write(Resources.Commands_Stop_StopFlowSynxSuccessfully);
            }
            else
            {
                _flowCtlLogger.Write(Resources.Commands_Uninstall_FlowSynxSystemIsRunning);
                return Task.CompletedTask;
            }
        }

        if (_processHandler.IsRunning(_location.ConsoleBinaryName, "."))
        {
            if (options.Force)
            {
                _processHandler.Terminate(_location.ConsoleBinaryName, ".");
                _flowCtlLogger.Write(Resources.Commands_Stop_StopConsoleSuccessfully);
            }
            else
            {
                _flowCtlLogger.Write(Resources.Commands_Uninstall_ConsoleIsRunning);
                return Task.CompletedTask;
            }
        }

        if (Directory.Exists(_location.DefaultFlowSynxDirectoryName))
            Directory.Delete(_location.DefaultFlowSynxDirectoryName, true);

        _flowCtlLogger.Write(Resources.Commands_Uninstall_UninstalledSuccessfully);
        return Task.CompletedTask;
    }

    private void RemoveDataDirectory(string? hostDataPath)
    {
        if (string.IsNullOrWhiteSpace(hostDataPath))
            return;

        var fullPath = Path.GetFullPath(hostDataPath);
        var root = Path.GetPathRoot(fullPath);
        if (!string.IsNullOrWhiteSpace(root) && string.Equals(fullPath.TrimEnd(Path.DirectorySeparatorChar), root.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
        {
            _flowCtlLogger.Write(Resources.Commands_Uninstall_SkipDataDeletion);
            return;
        }

        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, true);
            _flowCtlLogger.Write(string.Format(Resources.Commands_Uninstall_RemovedDataDirectory, fullPath));
        }
    }
}
