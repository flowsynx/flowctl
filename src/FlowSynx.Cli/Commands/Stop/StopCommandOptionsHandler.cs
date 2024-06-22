using System.Diagnostics;
using EnsureThat;
using FlowSynx.Cli.Services.Abstracts;

namespace FlowSynx.Cli.Commands.Stop;

internal class StopCommandOptionsHandler : ICommandOptionsHandler<StopCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;

    public StopCommandOptionsHandler(IOutputFormatter outputFormatter)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        _outputFormatter = outputFormatter;
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
            TerminateProcess("flowsynx", ".");
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