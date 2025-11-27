using System.Diagnostics;
using FlowCtl.Core.Services.Configuration;
using FlowCtl.Core.Services.Docker;
using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Stop;

internal class StopCommandOptionsHandler : ICommandOptionsHandler<StopCommandOptions>
{
    private const string DefaultContainerName = "flowsynx-engine";

    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;
    private readonly IDockerService _dockerService;
    private readonly IAppSettingsService _appSettingsService;

    public StopCommandOptionsHandler(
        IFlowCtlLogger flowCtlLogger,
        ILocation location,
        IDockerService dockerService,
        IAppSettingsService appSettingsService)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _location = location ?? throw new ArgumentNullException(nameof(location));
        _dockerService = dockerService ?? throw new ArgumentNullException(nameof(dockerService));
        _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
    }

    public async Task<int> HandleAsync(StopCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(StopCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (options.Docker)
            {
                await StopDockerAsync(cancellationToken);
                return;
            }

            TerminateProcess(_location.FlowSynxBinaryName, ".");
        }
        catch (Exception e)
        {
            _flowCtlLogger.WriteError(e.Message);
        }
    }

    private async Task StopDockerAsync(CancellationToken cancellationToken)
    {
        if (!await _dockerService.IsDockerAvailableAsync(cancellationToken))
        {
            _flowCtlLogger.WriteError(Resources.Commands_Init_DockerUnavailable);
            return;
        }

        var settings = await _appSettingsService.LoadAsync(cancellationToken);
        var containerName = string.IsNullOrWhiteSpace(settings.Docker.ContainerName)
            ? DefaultContainerName
            : settings.Docker.ContainerName;

        var running = await _dockerService.IsContainerRunningAsync(containerName, cancellationToken);
        if (!running)
        {
            _flowCtlLogger.Write(string.Format(Resources.Commands_Stop_DockerNotRunning, containerName));
            return;
        }

        var result = await _dockerService.StopContainerAsync(containerName, cancellationToken);
        if (!result.Success)
        {
            _flowCtlLogger.WriteError(string.IsNullOrWhiteSpace(result.Error)
                ? Resources.Commands_Stop_DockerStopFailed
                : result.Error);
            return;
        }

        _flowCtlLogger.Write(string.Format(Resources.Commands_Stop_DockerStoppedSuccessfully, containerName));
    }

    private void TerminateProcess(string processName, string machineAddress)
    {
        var processes = Process.GetProcessesByName(processName, machineAddress);

        if (processes.Length == 0) return;

        try
        {
            foreach (var process in processes)
                process.Kill();

            _flowCtlLogger.Write(Resources.Commands_Stop_StopFlowSynxSuccessfully);
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }
}
