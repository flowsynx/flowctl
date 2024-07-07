namespace FlowCtl.Commands.Init;

internal class InitCommandOptions : ICommandOptions
{
    public string? FlowSynxVersion { get; set; } = string.Empty;
    public string? DashboardVersion { get; set; } = string.Empty;
}