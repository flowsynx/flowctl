namespace FlowCtl.Commands.Config.Add;

internal class AddConfigCommandOptions : ICommandOptions
{
    public string Data { get; set; } = string.Empty;
    public string? DataFile { get; set; }
    public string? Address { get; set; } = string.Empty;
}