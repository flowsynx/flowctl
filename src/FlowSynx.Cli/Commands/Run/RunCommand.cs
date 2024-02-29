using System.CommandLine;
using System.Diagnostics;
using EnsureThat;
using FlowSynx.Cli.Common;
using FlowSynx.Cli.Formatter;
using FlowSynx.Logging;
using Spectre.Console;

namespace FlowSynx.Cli.Commands.Run;

internal class RunCommand : BaseCommand<RunCommandOptions, RunCommandOptionsHandler>
{
    public RunCommand() : base("run", "Run and execute the FlowSynx system on the current user profile")
    {
        var configFileOption = new Option<string>("--config-file", description: "FlowSynx configuration file");

        var enableHealthCheckOption = new Option<bool>("--enable-health-check", getDefaultValue: () => true,
            description: "Enable health checks for the FlowSynx");

        var enableLogOption = new Option<bool>("--enable-log", getDefaultValue: () => true,
            description: "Enable logging to records the details of events during FlowSynx running");

        var logLevelOption = new Option<LoggingLevel>("--log-level", getDefaultValue: () => LoggingLevel.Info,
            description: "The log verbosity to controls the amount of detail emitted for each event that is logged");

        var logFileOption = new Option<string?>("--log-file", description: "Log file path to store system logs information");

        AddOption(configFileOption);
        AddOption(enableHealthCheckOption);
        AddOption(enableLogOption);
        AddOption(logLevelOption);
        AddOption(logFileOption);
    }
}

internal class RunCommandOptions : ICommandOptions
{
    public string? ConfigFile { get; set; }
    public bool EnableHealthCheck { get; set; }
    public bool EnableLog { get; set; }
    public LoggingLevel LogLevel { get; set; }
    public string? LogFile { get; set; }
}

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
        var flowSynxPath = Path.Combine(PathHelper.DefaultFlowSynxDirectoryName, "engine");
        var flowSynxBinaryFile = PathHelper.LookupFlowSynxBinaryFilePath(flowSynxPath);
        if (!Path.Exists(flowSynxBinaryFile))
        {
            _outputFormatter.WriteError(Resources.FlowSynxEngineIsNotInstalled);
            return Task.CompletedTask;
        }

        var color = AnsiConsole.Foreground;
        var startInfo = new ProcessStartInfo(flowSynxBinaryFile)
        {
            Arguments = GetArgumentStr(options),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = new Process { StartInfo = startInfo };
        process.OutputDataReceived += OutputDataHandler;
        process.ErrorDataReceived += ErrorDataHandler;
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process?.WaitForExit();
        AnsiConsole.Foreground = color;

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