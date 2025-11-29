namespace FlowCtl.Core.Models.Docker;

public class DockerRunOptions
{
    public string ImageName { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public int HostPort { get; set; }
    public int ContainerPort { get; set; } = 6262;
    public string HostDataPath { get; set; } = string.Empty;
    public string ContainerDataPath { get; set; } = string.Empty;
    public bool Detached { get; set; } = true;
    public string? AdditionalArguments { get; set; } = string.Empty;
}
