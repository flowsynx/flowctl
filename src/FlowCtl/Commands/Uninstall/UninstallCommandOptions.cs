namespace FlowCtl.Commands.Uninstall;

internal class UninstallCommandOptions : ICommandOptions
{
    public bool Force { get; set; }
    public bool Docker { get; set; }
    public bool RemoveData { get; set; }
}
