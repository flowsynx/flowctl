namespace FlowCtl.Commands.Plugins.Install;

internal class InstallPluginCommandOptions : ICommandOptions
{
    /// <summary>
    /// FlowSynx plugin identifier (e.g. email-sender).
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Optional plugin version; defaults to the FlowSynx "latest" tag.
    /// </summary>
    public string? Version { get; set; }

    public string? Address { get; set; } = string.Empty;
}
