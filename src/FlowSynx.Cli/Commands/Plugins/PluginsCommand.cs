using System.CommandLine;
using FlowSynx.Cli.Commands.Plugins.Details;

namespace FlowSynx.Cli.Commands.Plugins;

internal class PluginsCommand : BaseCommand<PluginsCommandOptions, PluginsCommandOptionsHandler>
{
    public PluginsCommand() : base("plugins", "Display list and details of plugins supported by FlowSynx system")
    {
        var typeOption = new Option<string>(new[] { "-t", "--type" },
            description: "The namespace of plugin (e.g., Storage)");

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: "The address that specify a http-based address to connect on remote FlowSynx system");

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: "Formatting CLI output");

        AddOption(typeOption);
        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new PluginDetailsCommand());
    }
}