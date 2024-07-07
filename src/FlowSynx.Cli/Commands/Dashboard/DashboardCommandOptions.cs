using FlowSynx.Logging;

namespace FlowCtl.Commands.Dashboard;

internal class DashboardCommandOptions : ICommandOptions
{
    public string? Address { get; set; } = string.Empty;
}