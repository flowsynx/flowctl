using FlowCtl.Core.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Plugins.Delete;

internal class DeletePluginCommand : BaseCommand<DeletePluginCommandOptions, DeletePluginCommandOptionsHandler>
{
    public DeletePluginCommand() : base("delete", Resources.ConnectorDetailsCommandDescription)
    {
        var typeOption = new Option<string>(new[] { "-t", "--type" },
            description: Resources.ConnectorDetailsCommandTypeOption) { IsRequired = true };

        var versionOption = new Option<string>(new[] { "-v", "--version" },
            description: Resources.ConnectorDetailsCommandTypeOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(typeOption);
        AddOption(versionOption);
        AddOption(addressOption);
    }
}