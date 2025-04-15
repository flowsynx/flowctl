using System.Diagnostics;
using EnsureThat;
using FlowCtl.Core.Logger;
using FlowCtl.Core.Services;

namespace FlowCtl.Commands.Stop;

internal class StopCommandOptionsHandler : ICommandOptionsHandler<StopCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;

    public StopCommandOptionsHandler(IFlowCtlLogger flowCtlLogger, ILocation location)
    {
        EnsureArg.IsNotNull(flowCtlLogger, nameof(flowCtlLogger));
        EnsureArg.IsNotNull(location, nameof(location));
        _flowCtlLogger = flowCtlLogger;
        _location = location;
    }

    public async Task<int> HandleAsync(StopCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute();
        return 0;
    }

    private Task Execute()
    {
        try
        {
            TerminateProcess(_location.FlowSynxBinaryName, ".");
        }
        catch (Exception e)
        {
            _flowCtlLogger.WriteError(e.Message);
        }
        return Task.CompletedTask;
    }

    private void TerminateProcess(string processName, string machineAddress)
    {
        var processes = Process.GetProcessesByName(processName, machineAddress);

        if (processes.Length == 0) return;

        try
        {
            foreach (var process in processes)
                process.Kill();

            _flowCtlLogger.Write(Resources.StopCommandFlowSynxStopped);
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }
}