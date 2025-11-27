namespace FlowCtl.Core.Models.Configuration;

public class AppSettings
{
    public DeploymentMode DeploymentMode { get; set; } = DeploymentMode.Binary;
    public DockerSettings Docker { get; set; } = new();
    public BinarySettings Binary { get; set; } = new();
}

public class DockerSettings
{
    public string ImageName { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public int Port { get; set; } = 6262;
    public string HostDataPath { get; set; } = string.Empty;
    public string ContainerDataPath { get; set; } = "/app/data";
}

public class BinarySettings
{
    public string? Version { get; set; }
}

public enum DeploymentMode
{
    Binary,
    Docker
}
