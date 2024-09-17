using System.CommandLine;
using FlowSynx.Logging;

namespace FlowCtl.Commands.Logs;

internal class LogsCommand : BaseCommand<LogsCommandOptions, LogsCommandOptionsHandler>
{
    public LogsCommand() : base("logs", Resources.LogsCommandDescription)
    {
        var fieldsOption = new Option<string[]?>(new[] { "-fd", "--fields" },
            getDefaultValue: Array.Empty<string>,
            description: Resources.CommandIncludeOption);

        var filterOption = new Option<string?>(new[] { "-f", "--filter" },
            getDefaultValue: () => string.Empty,
            description: Resources.CommandExcludeOption);

        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" },
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var sortOption = new Option<string?>(new[] { "-s", "--sort" },
            getDefaultValue: () => string.Empty,
            description: Resources.CommandSortingOption);

        var limitOption = new Option<string?>(new[] { "-l", "--limit" },
            getDefaultValue: () => string.Empty,
            description: Resources.LogsCommandLogLevelOption);

        var exportPathOption = new Option<string?>(new[] { "-et", "--export-to" },
            getDefaultValue: () => string.Empty,
            description: Resources.ReadCommandSaveToOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            getDefaultValue: () => string.Empty,
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