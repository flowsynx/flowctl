using System.Diagnostics;
using EnsureThat;
using FlowCtl.Services.Abstracts;

namespace FlowCtl.Commands.Stop;

internal class StopCommandOptionsHandler : ICommandOptionsHandler<StopCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ILocation _location;

    public StopCommandOptionsHandler(IOutputFormatter outputFormatter, ILocation location)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(location, nameof(location));
        _outputFormatter = outputFormatter;
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
            _outputFormatter.WriteError(e.Message);
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

            _outputFormatter.Write(Resources.StopCommandFlowSynxStopped);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}