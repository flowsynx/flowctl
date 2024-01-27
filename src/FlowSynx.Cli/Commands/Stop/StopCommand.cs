using System.Diagnostics;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Cli.Services;

namespace FlowSynx.Cli.Commands.Stop;

internal class StopCommand : BaseCommand<StopCommandOptions, StopCommandOptionsHandler>
{
    public StopCommand() : base("stop", "Stop the FlowSynx system which running on the current user profile")
    {

    }
}

internal class StopCommandOptions : ICommandOptions
{

}

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
        await Execute(options, cancellationToken);
        return 0;
    }

    private Task Execute(StopCommandOptions options, CancellationToken cancellationToken)
    {
        TerminateProcess("FlowSynx", ".");
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

            _outputFormatter.Write("The FlowSynx system was stopped successfully.");
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}