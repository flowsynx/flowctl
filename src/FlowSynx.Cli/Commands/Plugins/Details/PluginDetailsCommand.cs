using System.CommandLine;

namespace FlowCtl.Commands.Plugins.Details;

internal class PluginDetailsCommand : BaseCommand<PluginDetailsCommandOptions, PluginDetailsCommandOptionsHandler>
{
    public PluginDetailsCommand() : base("details", Resources.PluginDetailsCommandDescription)
    {
        var nameOption = new Option<Guid>(new[] { "-i", "--id" },
            description: Resources.PluginDetailsCommandIdOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputFormatOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(nameOption);
        AddOption(addressOption);
        AddOption(outputFormatOption);
    }
}