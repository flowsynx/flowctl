using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;
using System.Diagnostics;

namespace FlowCtl.Commands.Console;

internal class ConsoleCommandOptionsHandler : ICommandOptionsHandler<ConsoleCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;

    public ConsoleCommandOptionsHandler(
        IFlowCtlLogger flowCtlLogger,
        ILocation location)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _location = location;
    }

    public async Task<int> HandleAsync(ConsoleCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options);
        return 0;
    }

    private Task Execute(ConsoleCommandOptions options)
    {
        try
        {
            var dashboardPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "console");
            var dashboardBinaryFile = _location.LookupConsoleBinaryFilePath(dashboardPath);
            if (!Path.Exists(dashboardBinaryFile))
            {
                _flowCtlLogger.WriteError(Resources.Command_Console_ConsleIsNotInstalled);
                return Task.CompletedTask;
            }

            var startInfo = new ProcessStartInfo(dashboardBinaryFile)
            {
                Arguments = GetArgumentStr(options),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = dashboardPath
            };

            var process = new Process { StartInfo = startInfo };

            if (!options.Background)
            {
                process.OutputDataReceived += OutputDataHandler;
                process.ErrorDataReceived += ErrorDataHandler;
            }

            var processStarted = process.Start();

            if (!options.Background)
            {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process?.WaitForExit();
            }
        }
        catch (Exception e)
        {
            _flowCtlLogger.WriteError(e.Message);
        }
        return Task.CompletedTask;
    }

    private string GetArgumentStr(ConsoleCommandOptions options)
    {
        var argList = new List<string>();

        if (!string.IsNullOrEmpty(options.Address))
            argList.Add($"--address {options.Address}");

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
}