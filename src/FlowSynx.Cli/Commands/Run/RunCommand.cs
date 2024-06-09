using System.CommandLine;
using FlowSynx.Logging;

namespace FlowSynx.Cli.Commands.Run;

internal class RunCommand : BaseCommand<RunCommandOptions, RunCommandOptionsHandler>
{
    public RunCommand() : base("run", "Run and execute the FlowSynx system on the current user profile")
    {
        var configFileOption = new Option<string>("--config-file", 
            description: "FlowSynx configuration file");

        var enableHealthCheckOption = new Option<bool>("--enable-health-check", 
            getDefaultValue: () => true,
            description: "Enable health checks for the FlowSynx");

        var enableLogOption = new Option<bool>("--enable-log", 
            getDefaultValue: () => true,
            description: "Enable logging to records the details of events during FlowSynx running");

        var logLevelOption = new Option<LoggingLevel>("--log-level", 
            getDefaultValue: () => LoggingLevel.Info,
            description: "The log verbosity to controls the amount of detail emitted for each event that is logged");

        var logFileOption = new Option<string?>("--log-file", 
            description: "Log file path to store system logs information");

        var openApiOption = new Option<bool>(new[] { "--open-api" }, getDefaultValue: () => false,
            description: "Enable OpenApi specification for FlowSynx");

        AddOption(configFileOption);
        AddOption(enableHealthCheckOption);
        AddOption(enableLogOption);
        AddOption(logLevelOption);
        AddOption(logFileOption);
        AddOption(openApiOption);
    }
}