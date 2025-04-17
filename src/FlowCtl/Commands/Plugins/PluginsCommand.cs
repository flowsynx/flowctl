using System.CommandLine;
using FlowCtl.Commands.Plugins.Add;
using FlowCtl.Commands.Plugins.Delete;
using FlowCtl.Commands.Plugins.Details;
using FlowCtl.Commands.Plugins.Update;
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

        AddCommand(new AddPluginCommand());
        AddCommand(new DeletePluginCommand());
        AddCommand(new PluginDetailsCommand());
        AddCommand(new UpdatePluginCommand());
    }
}