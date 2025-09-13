namespace FlowCtl.Commands.Run;

internal class RunCommandOptions : ICommandOptions
{
    public bool Background { get; set; } = false;
}