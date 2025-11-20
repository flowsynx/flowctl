namespace FlowCtl.Core.Models.Docker;

public class DockerCommandResult
{
    public int ExitCode { get; set; }
    public string Output { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;

    public bool Success => ExitCode == 0;
}
