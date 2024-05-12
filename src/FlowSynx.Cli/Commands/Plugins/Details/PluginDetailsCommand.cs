using System.CommandLine;

namespace FlowSynx.Cli.Commands.Plugins.Details;

internal class PluginDetailsCommand : BaseCommand<PluginDetailsCommandOptions, PluginDetailsCommandOptionsHandler>
{
    public PluginDetailsCommand() : base("details", "Display details about a particular plugin")
    {
        var nameOption = new Option<Guid>(new[] { "-i", "--id" },
            description: "The identifier (Id) of the plugin") { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: "The address that specify a http-based address to connect on remote FlowSynx system");

        var outputFormatOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: "Formatting CLI output");

        AddOption(nameOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);
    }
}