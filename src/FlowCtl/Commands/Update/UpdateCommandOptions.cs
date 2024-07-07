namespace FlowCtl.Commands.Update;

internal class UpdateCommandOptions : ICommandOptions
{
    public bool Force { get; set; } = false;
    public string? FlowSynxVersion { get; set; } = string.Empty;
    public string? DashboardVersion { get; set; } = string.Empty;
}