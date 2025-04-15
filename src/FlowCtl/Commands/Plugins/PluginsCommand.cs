using System.CommandLine;
using FlowCtl.Commands.Plugins.Details;
using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Plugins;

internal class PluginsCommand : BaseCommand<PluginsCommandOptions, PluginsCommandOptionsHandler>
{
    public PluginsCommand() : base("plugins", Resources.ConnectorsCommandDescription)
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<OutputType>(new[] { "-o", "--output" },
            getDefaultValue: () => OutputType.Json,
            description: Resources.CommandOutputOption);

        AddOption(addressOption);
        AddOption(outputOption);

        AddCommand(new PluginDetailsCommand());
    }
}