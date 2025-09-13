namespace FlowCtl.Commands.Console;

internal class ConsoleCommandOptions : ICommandOptions
{
    public bool Background { get; set; } = false;
    public string? Address { get; set; } = string.Empty;
}