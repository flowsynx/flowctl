using FlowSynx.Logging;

namespace FlowSynx.Cli.Commands.Run;

internal class RunCommandOptions : ICommandOptions
{
    public string? ConfigFile { get; set; }
    public bool EnableHealthCheck { get; set; }
    public bool EnableLog { get; set; }
    public LoggingLevel LogLevel { get; set; }
    public string? LogFile { get; set; }
    public bool OpenApi { get; set; }
}