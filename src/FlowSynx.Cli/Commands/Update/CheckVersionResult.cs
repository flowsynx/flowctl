namespace FlowCtl.Commands.Update;

internal class CheckVersionResult
{
    public bool IsUpdateAvailable { get; set; }
    public string Version { get; set; } = string.Empty;
}