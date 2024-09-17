using System.CommandLine;

namespace FlowCtl.Commands.Config.Delete;

internal class DeleteConfigCommand : BaseCommand<DeleteConfigCommandOptions, DeleteConfigCommandOptionsHandler>
{
    public DeleteConfigCommand() : base("delete", Resources.DeleteConfigCommandDescription)
    {
        var filterOption = new Option<string?>(new[] { "-f", "--filter" },
            description: Resources.CommandFilterOption);

        var caseSensitiveOption = new Option<bool?>(new[] { "-c", "--case-sensitive" },
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var sortOption = new Option<string?>(new[] { "-s", "--sort" },
            description: Resources.CommandSortOption);

        var limitOption = new Option<string?>(new[] { "-l", "--limit" },
            description: Resources.LogsCommandLogLevelOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(filterOption);
        AddOption(caseSensitiveOption);
        AddOption(sortOption);
        AddOption(limitOption);
        AddOption(caseSensitiveOption);
        AddOption(addressOption);
    }
}