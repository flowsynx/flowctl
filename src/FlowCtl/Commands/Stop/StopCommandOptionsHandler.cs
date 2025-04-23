using System.Diagnostics;
using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Stop;

internal class StopCommandOptionsHandler : ICommandOptionsHandler<StopCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;

    public StopCommandOptionsHandler(IFlowCtlLogger flowCtlLogger, ILocation location)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _location = location ?? throw new ArgumentNullException(nameof(location));
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

            _flowCtlLogger.Write(Resources.Commands_Stop_StopSuccessfully);
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }
}