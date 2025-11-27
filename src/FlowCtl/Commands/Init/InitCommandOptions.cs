namespace FlowCtl.Commands.Init;

internal class InitCommandOptions : ICommandOptions
{
    public string? FlowSynxVersion { get; set; } = string.Empty;
    public string? ConsoleVersion { get; set; } = string.Empty;
    public bool Docker { get; set; }
    public string? ContainerName { get; set; }
    public int Port { get; set; } = 6262;
    public string? Mount { get; set; }
    public string? ContainerPath { get; set; } = "/app/data";
}
