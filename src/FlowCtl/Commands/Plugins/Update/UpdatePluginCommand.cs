using System.CommandLine;

namespace FlowCtl.Commands.Plugins.Update;

internal class UpdatePluginCommand : BaseCommand<UpdatePluginCommandOptions, UpdatePluginCommandOptionsHandler>
{
    public UpdatePluginCommand() : base("update", Resources.ConnectorDetailsCommandDescription)
    {
        var typeOption = new Option<string>(new[] { "-t", "--type" },
            description: Resources.ConnectorDetailsCommandTypeOption) { IsRequired = true };

        var oldVersionOption = new Option<string>(new[] { "-o", "--old-version" },
            description: Resources.ConnectorDetailsCommandTypeOption) { IsRequired = true };

        var newVersionOption = new Option<string>(new[] { "-n", "--new-version" },
            description: Resources.ConnectorDetailsCommandTypeOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(typeOption);
        AddOption(oldVersionOption);
        AddOption(newVersionOption);
        AddOption(addressOption);
    }
}