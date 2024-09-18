using System.CommandLine;
using FlowSynx.Logging;

namespace FlowCtl.Commands.Logs;

internal class LogsCommand : BaseCommand<LogsCommandOptions, LogsCommandOptionsHandler>
{
    public LogsCommand() : base("logs", Resources.LogsCommandDescription)
    {
        var fieldsOption = new Option<string[]?>(new[] { "-f", "--fields" },
            description: Resources.CommandFieldOption);

        var filterOption = new Option<string?>(new[] { "+f", "--filter" },
            description: Resources.CommandFilterOption);

        var caseSensitiveOption = new Option<bool?>(new[] { "-c", "--case-sensitive" },
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var sortOption = new Option<string?>(new[] { "-s", "--sort" },
            description: Resources.CommandSortOption);

        var limitOption = new Option<string?>(new[] { "-l", "--limit" },
            description: Resources.CommandLimitOption);

        var exportPathOption = new Option<string?>(new[] { "-e", "--export-to" },
            description: Resources.ReadCommandSaveToOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" },
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(fieldsOption);
        AddOption(filterOption);
        AddOption(caseSensitiveOption);
        AddOption(sortOption);
        AddOption(limitOption);
        AddOption(exportPathOption);
        AddOption(addressOption);
        AddOption(outputOption);
    }
}