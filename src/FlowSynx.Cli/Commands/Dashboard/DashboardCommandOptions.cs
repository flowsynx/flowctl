using FlowSynx.Logging;

namespace FlowSynx.Cli.Commands.Dashboard;

internal class DashboardCommandOptions : ICommandOptions
{
    public string? Address { get; set; } = string.Empty;
}