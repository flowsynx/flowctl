using System.CommandLine;
using System.Diagnostics;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.IO.Serialization;
using FlowSynx.Logging;
using Spectre.Console;

namespace FlowSynx.Cli.Commands.Execute;

internal class InitCommand : BaseCommand<InitCommandOptions, InitCommandOptionsHandler>
{
    public InitCommand() : base("run", "Run FlowSync system")
    {
        var configOption = new Option<string>(new[] { "--config-file" }, description: "FlowSync configuration file");
        var enableHealthCheckOption = new Option<bool>(new[] { "--enable-health-check" }, getDefaultValue: () => true, description: "Enable health checks for the FlowSync");
        var enableLogOption = new Option<bool>(new[] { "--enable-log" }, getDefaultValue: () => true, description: "Enable logging to records the details of events during FlowSync running");
        var logLevelOption = new Option<LoggingLevel>(new[] { "--log-level" }, getDefaultValue: () => LoggingLevel.Info, description: "The log verbosity to controls the amount of detail emitted for each event that is logged");
        var retryOption = new Option<int>(new[] { "--retry" }, getDefaultValue: () => 3, description: "The number of times FlowSync needs to try to receive data if there is a connection problem");

        AddOption(configOption);
        AddOption(enableHealthCheckOption);
        AddOption(enableLogOption);
        AddOption(logLevelOption);
        AddOption(retryOption);
    }
}

internal class InitCommandOptions : ICommandOptions
{
    public string? ConfigFile { get; set; }
    public bool EnableHealthCheck { get; set; }
    public bool EnableLog { get; set; }
    public LoggingLevel LogLevel { get; set; }
    public bool Retry { get; set; }
}

internal class InitCommandOptionsHandler : ICommandOptionsHandler<InitCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IEnvironmentManager _environmentManager;
    private readonly ISerializer _serializer;

    public InitCommandOptionsHandler(IOutputFormatter outputFormatter, IEnvironmentManager environmentManager, 
        ISerializer serializer)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(environmentManager, nameof(environmentManager));
        EnsureArg.IsNotNull(serializer, nameof(serializer));

        _outputFormatter = outputFormatter;
        _environmentManager = environmentManager;
        _serializer = serializer;
    }

    public async Task<int> HandleAsync(InitCommandOptions options, CancellationToken cancellationToken)
    {
        await RunFlowSync(options, cancellationToken);
        return 0;
    }

    private Task RunFlowSync(InitCommandOptions options, CancellationToken cancellationToken)
    {
        var flowSyncPath = _environmentManager.Get(EnvironmentVariables.FlowsynxPath);
        if (string.IsNullOrEmpty(flowSyncPath))
        {
            _outputFormatter.WriteError(@"FlowSynx engine is not installed. Please run the 'synx install -h' command to see the details.");
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

    private string GetArgumentStr(InitCommandOptions options)
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
        if (outLine.Data != null) _outputFormatter.Write(outLine.Data);
    }

    private void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (outLine.Data != null) _outputFormatter.WriteError(outLine.Data);
    }
}