using System.CommandLine;

namespace FlowCtl.Commands.Plugins.Details;

internal class PluginDetailsCommand : BaseCommand<PluginDetailsCommandOptions, PluginDetailsCommandOptionsHandler>
{
    public PluginDetailsCommand() : base("details", Resources.PluginDetailsCommandDescription)
    {
        var typeOption = new Option<string>(new[] { "-t", "--type" },
            description: Resources.PluginDetailsCommandTypeOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputFormatOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(typeOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);
    }
}