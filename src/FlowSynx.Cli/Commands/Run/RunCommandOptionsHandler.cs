using System.Diagnostics;
using EnsureThat;
using FlowSynx.Cli.Services.Abstracts;
using FlowSynx.IO.Serialization;

namespace FlowSynx.Cli.Commands.Run;

internal class RunCommandOptionsHandler : ICommandOptionsHandler<RunCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ILocation _location;
    private readonly ISerializer _serializer;

    public RunCommandOptionsHandler(IOutputFormatter outputFormatter, ILocation location,
        ISerializer serializer)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(location, nameof(location));
        EnsureArg.IsNotNull(serializer, nameof(serializer));

        _outputFormatter = outputFormatter;
        _location = location;
        _serializer = serializer;
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

        string configFile;
        if (!string.IsNullOrEmpty(options.ConfigFile))
        {
            configFile = options.ConfigFile;
        }
        else
        {
            configFile = Path.Combine(_location.DefaultFlowSynxDirectoryName, "configuration.json");
            if (!File.Exists(configFile))
                File.WriteAllText(configFile, _serializer.Serialize(new { }));
        }

        argList.Add($"--config-file \"{configFile}\"");

        argList.Add($"--enable-health-check {options.EnableHealthCheck}");
        argList.Add($"--enable-log {options.EnableLog}");
        argList.Add($"--log-level {options.LogLevel}");

        if (!string.IsNullOrEmpty(options.LogFile))
            argList.Add($"--log-file {options.LogFile}");

        argList.Add($"--open-api {options.OpenApi}");

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