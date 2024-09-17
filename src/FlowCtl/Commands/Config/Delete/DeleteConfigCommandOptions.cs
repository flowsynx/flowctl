namespace FlowCtl.Commands.Config.Delete;

internal class DeleteConfigCommandOptions : ICommandOptions
{
    public string? Filter { get; set; } = string.Empty;
    public bool? CaseSensitive { get; set; } = false;
    public string? Sort { get; set; } = string.Empty;
    public string? Limit { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
}