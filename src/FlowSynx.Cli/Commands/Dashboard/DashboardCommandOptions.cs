using FlowSynx.Logging;

namespace FlowSynx.Cli.Commands.Dashboard;

internal class DashboardCommandOptions : ICommandOptions
{
    public string? Url { get; set; } = string.Empty;
}