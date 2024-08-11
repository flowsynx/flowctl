using System.CommandLine;
using FlowSynx.Logging;

namespace FlowCtl.Commands.Logs;

internal class LogsCommand : BaseCommand<LogsCommandOptions, LogsCommandOptionsHandler>
{
    public LogsCommand() : base("logs", Resources.LogsCommandDescription)
    {
        var includeOption = new Option<string?>(new[] { "-i", "--include" },
            description: Resources.CommandIncludeOption);

        var excludeOption = new Option<string?>(new[] { "-e", "--exclude" },
            description: Resources.CommandExcludeOption);

        var minAgeOption = new Option<string?>(new[] { "-ma", "--min-age" },
            description: Resources.CommandMinAgeOption);

        var maxAgeOption = new Option<string?>(new[] { "+ma", "--max-age" },
            description: Resources.CommandMaxAgeOption);

        var caseSensitiveOption = new Option<bool?>(new[] { "-cs", "--case-sensitive" },
            getDefaultValue: () => false,
            description: Resources.CommandCaseSensitiveOption);

        var maxResultsOption = new Option<int?>(new[] { "-mr", "--max-results" },
            description: Resources.CommandMaxResultsOption);

        var sortingOption = new Option<string?>(new[] { "-so", "--sorting" },
            description: Resources.CommandSortingOption);

        var logLevelOption = new Option<string?>(new[] { "-l", "--level" },
            description: Resources.LogsCommandLogLevelOption);

        var exportPathOption = new Option<string?>(new[] { "-et", "--export-to" },
            description: Resources.ReadCommandSaveToOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" },
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(includeOption);
        AddOption(excludeOption);
        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(caseSensitiveOption);
        AddOption(maxResultsOption);
        AddOption(sortingOption);
        AddOption(logLevelOption);
        AddOption(exportPathOption);
        AddOption(addressOption);
        AddOption(outputOption);
    }
}