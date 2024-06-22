using System.CommandLine;
using FlowSynx.Logging;

namespace FlowSynx.Cli.Commands.Run;

internal class RunCommand : BaseCommand<RunCommandOptions, RunCommandOptionsHandler>
{
    public RunCommand() : base("run", Resources.RunCommandDescription)
    {
        var configFileOption = new Option<string>("--config-file", 
            description: Resources.RunCommandConfigFileOption);

        var enableHealthCheckOption = new Option<bool>("--enable-health-check", 
            getDefaultValue: () => true,
            description: Resources.RunCommandEnableHealthCheckOption);

        var enableLogOption = new Option<bool>("--enable-log", 
            getDefaultValue: () => true,
            description: Resources.RunCommandEnableLogOption);

        var logLevelOption = new Option<LoggingLevel>("--log-level", 
            getDefaultValue: () => LoggingLevel.Info,
            description: Resources.RunCommandLogLevelOption);

        var logFileOption = new Option<string?>("--log-file", 
            description: Resources.RunCommandLogFileOption);

        var openApiOption = new Option<bool>(new[] { "--open-api" }, getDefaultValue: () => false,
            description: Resources.RunCommandOpenApiOption);

        AddOption(configFileOption);
        AddOption(enableHealthCheckOption);
        AddOption(enableLogOption);
        AddOption(logLevelOption);
        AddOption(logFileOption);
        AddOption(openApiOption);
    }
}