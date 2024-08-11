namespace FlowCtl.Commands.Config.Delete;

internal class DeleteConfigCommandOptions : ICommandOptions
{
    public string? Include { get; set; } = string.Empty;
    public string? Exclude { get; set; } = string.Empty;
    public string? MinAge { get; set; } = string.Empty;
    public string? MaxAge { get; set; } = string.Empty;
    public bool? CaseSensitive { get; set; } = false;
    public string? Address { get; set; } = string.Empty;
}