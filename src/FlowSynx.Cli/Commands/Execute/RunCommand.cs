using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Cli.Services;
using FlowSynx.Logging;
using Spectre.Console;

namespace FlowSynx.Cli.Commands.Execute;

internal class RunCommand : BaseCommand<RunCommandOptions, RunCommandOptionsHandler>
{
    public RunCommand() : base("run", "Run FlowSync system")
    {
        var configFileOption = new Option<string>(new[] { "--config-file" }, description: "FlowSynx configuration file");

        var enableHealthCheckOption = new Option<bool>(new[] { "--enable-health-check" }, getDefaultValue: () => true,
            description: "Enable health checks for the FlowSynx");

        var enableLogOption = new Option<bool>(new[] { "--enable-log" }, getDefaultValue: () => true,
            description: "Enable logging to records the details of events during FlowSynx running");

        var logLevelOption = new Option<LoggingLevel>(new[] { "--log-level" }, getDefaultValue: () => LoggingLevel.Info,
            description: "The log verbosity to controls the amount of detail emitted for each event that is logged");

        var logFileOption = new Option<string?>(new[] { "--log-file" },
            description: "The log verbosity to controls the amount of detail emitted for each event that is logged");

        AddOption(configFileOption);
        AddOption(enableHealthCheckOption);
        AddOption(enableLogOption);
        AddOption(logLevelOption);
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
    private readonly ILocation _location;

    public RunCommandOptionsHandler(IOutputFormatter outputFormatter, ILocation location)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(location, nameof(location));
        _outputFormatter = outputFormatter;
        _location = location;
    }

    private string UserProfilePath => System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
    private string DefaultFlowSynxDirName => Path.Combine(UserProfilePath, ".flowsynx");

    public async Task<int> HandleAsync(RunCommandOptions options, CancellationToken cancellationToken)
    {
        await RunFlowSynx(options, cancellationToken);
        return 0;
    }

    private Task RunFlowSynx(RunCommandOptions options, CancellationToken cancellationToken)
    {
        var flowSynxPath = Path.Combine(UserProfilePath, DefaultFlowSynxDirName, "engine");
        var flowSynxBinaryFile = LookupBinaryFilePath(flowSynxPath);
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

    private string LookupBinaryFilePath(string path)
    {
        var binFileName = "FlowSynx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";
        
        return Path.Combine(path, binFileName);
    }
}