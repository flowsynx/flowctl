using System.CommandLine;
using FlowSynx.Cli.Commands.Plugins.Details;

namespace FlowSynx.Cli.Commands.Plugins;

internal class PluginsCommand : BaseCommand<PluginsCommandOptions, PluginsCommandOptionsHandler>
{
    public PluginsCommand() : base("plugins", "Display list and details of plugins supported by FlowSynx system")
    {
        var typeOption = new Option<string>("--type", "The namespace of plugin (like Storage)");
        var outputOption = new Option<Output>("--output", getDefaultValue: () => Output.Json, "Formatting CLI output");

        AddOption(typeOption);
        AddOption(outputOption);

        AddCommand(new PluginDetailsCommand());
    }
}