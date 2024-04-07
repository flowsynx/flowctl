using System.CommandLine;

namespace FlowSynx.Cli.Commands.Plugins.Details;

internal class PluginDetailsCommand : BaseCommand<DetailsPluginCommandOptions, DetailsPluginCommandOptionsHandler>
{
    public PluginDetailsCommand() : base("details", "Display details about a particular plugin")
    {
        var nameOption = new Option<Guid>("--id", "The identifier (Id) of the plugin") { IsRequired = true };
        var outputFormatOption = new Option<Output>("--output", getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(nameOption);
        AddOption(outputFormatOption);
    }
}