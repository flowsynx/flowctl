using System.Diagnostics;
using EnsureThat;
using FlowSynx.Cli.Common;
using FlowSynx.Cli.Services;

namespace FlowSynx.Cli.Commands.Run;

internal class RunCommandOptionsHandler : ICommandOptionsHandler<RunCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;

    public RunCommandOptionsHandler(IOutputFormatter outputFormatter)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        _outputFormatter = outputFormatter;
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
            var flowSynxPath = Path.Combine(PathHelper.DefaultFlowSynxBinaryDirectoryName, "engine");
            var flowSynxBinaryFile = PathHelper.LookupFlowSynxBinaryFilePath(flowSynxPath);
            if (!Path.Exists(flowSynxBinaryFile))
            {
                _outputFormatter.WriteError(Resources.FlowSynxEngineIsNotInstalled);
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
            _outputFormatter.WriteError(e.Message);
        }
        return Task.CompletedTask;
    }

    private string GetArgumentStr(RunCommandOptions options)
    {
        var argList = new List<string>();

        if (!string.IsNullOrEmpty(options.ConfigFile))
            argList.Add($"--config-file {options.ConfigFile}");

        argList.Add($"--enable-health-check {options.EnableHealthCheck}");
        argList.Add($"--enable-log {options.EnableLog}");
        argList.Add($"--log-level {options.LogLevel}");

        if (!string.IsNullOrEmpty(options.LogFile))
            argList.Add($"--log-file {options.LogFile}");

        return argList.Count == 0 ? string.Empty : string.Join(' ', argList);
    }

    private void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (outLine.Data != null) _outputFormatter.Write(outLine.Data);
    }

    private void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (outLine.Data != null) _outputFormatter.WriteError(outLine.Data);
    }
}