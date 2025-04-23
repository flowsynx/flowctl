using System.Diagnostics;
using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Run;

internal class RunCommandOptionsHandler : ICommandOptionsHandler<RunCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;

    public RunCommandOptionsHandler(IFlowCtlLogger flowCtlLogger, ILocation location)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _location = location ?? throw new ArgumentNullException(nameof(location));
    }

    public async Task<int> HandleAsync(RunCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options);
        return 0;
    }

    private Task Execute(RunCommandOptions options)
    {
        try
        {
            var flowSynxPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "engine");
            var flowSynxBinaryFile = _location.LookupFlowSynxBinaryFilePath(flowSynxPath);
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
            process.OutputDataReceived += OutputDataHandler;
            process.ErrorDataReceived += ErrorDataHandler;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process?.WaitForExit();
        }
        catch (Exception e)
        {
            _flowCtlLogger.WriteError(e.Message);
        }
        return Task.CompletedTask;
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
}