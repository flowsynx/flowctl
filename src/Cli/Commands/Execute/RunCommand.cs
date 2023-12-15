using System.CommandLine;
using System.Diagnostics;
using Cli.Extensions;
using Cli.Serialization;
using Cli.Services;
using Spectre.Console;

namespace Cli.Commands.Execute;

internal class RunCommand : BaseCommand<RunCommandOptions, RunCommandOptionsHandler>
{
    public RunCommand() : base("run", "Run FlowSync system")
    {
        var configOption = new Option<string>(new[] { "-c", "--config-file" }, description: "FlowSync configuration file");
        var enableHealthCheckOption = new Option<bool>(new[] { "-H", "--enable-health-check" }, getDefaultValue: () => true, description: "Enable health checks for the FlowSync");
        var enableLogOption = new Option<bool>(new[] { "-L", "--enable-log" }, getDefaultValue: () => true, description: "Enable logging to records the details of events during FlowSync running");
        var logLevelOption = new Option<AppLogLevel>(new[] { "-l", "--log-level" }, getDefaultValue: () => AppLogLevel.Information, description: "The log verbosity to controls the amount of detail emitted for each event that is logged");
        var retryOption = new Option<int>(new[] { "-r", "--retry" }, getDefaultValue: () => 3, description: "The number of times FlowSync needs to try to receive data if there is a connection problem");

        AddOption(configOption);
        AddOption(enableHealthCheckOption);
        AddOption(enableLogOption);
        AddOption(logLevelOption);
        AddOption(retryOption);
    }
}

internal class RunCommandOptions : ICommandOptions
{
    public string? ConfigFile { get; set; }
    public bool EnableHealthCheck { get; set; }
    public bool EnableLog { get; set; }
    public AppLogLevel LogLevel { get; set; }
    public bool Retry { get; set; }
}

internal class RunCommandOptionsHandler : ICommandOptionsHandler<RunCommandOptions>
{
    private readonly IAnsiConsole _console;
    private readonly IEnvironmentVariablesManager _environmentVariablesManager;
    private readonly ISerializer _serializer;

    public RunCommandOptionsHandler(IAnsiConsole console, IEnvironmentVariablesManager environmentVariablesManager, 
        ISerializer serializer)
    {
        _console = console;
        _environmentVariablesManager = environmentVariablesManager;
        _serializer = serializer;
    }

    public async Task<int> HandleAsync(RunCommandOptions options, CancellationToken cancellationToken)
    {
        await RunFlowSync(options, cancellationToken);
        return 0;
    }

    private Task RunFlowSync(RunCommandOptions options, CancellationToken cancellationToken)
    {
        var flowSyncPath = _environmentVariablesManager.Get("FLOWSYNC");
        if (string.IsNullOrEmpty(flowSyncPath))
        {
            _console.WriteError(@"FlowSync engine is not installed. Please run the 'fs-cli install -h' command to see the details.");
            return Task.CompletedTask;
        }

        var color = AnsiConsole.Foreground;
        var startInfo = new ProcessStartInfo(Path.Combine(flowSyncPath, "FlowSync.exe"))
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

        return argList.Count == 0 ? string.Empty : string.Join(' ', argList);
    }
    
    private void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (outLine.Data != null) _console.WriteText(outLine.Data);
    }

    private void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (outLine.Data != null) _console.WriteError(outLine.Data);
    }
}