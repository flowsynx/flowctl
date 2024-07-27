using System.CommandLine;
using FlowSynx.Logging;

namespace FlowCtl.Commands.Logs;

internal class LogsCommand : BaseCommand<LogsCommandOptions, LogsCommandOptionsHandler>
{
    public LogsCommand() : base("logs", Resources.ConfigCommandDescription)
    {
        var minAgeOption = new Option<string?>(new[] { "-ma", "--min-age" },
            description: Resources.CommandMinAgeOption);

        var maxAgeOption = new Option<string?>(new[] { "+ma", "--max-age" },
            description: Resources.CommandMaxAgeOption);

        var logLevelOption = new Option<LoggingLevel>(new[] { "-l", "--level" },
            getDefaultValue: () => LoggingLevel.Info,
            description: Resources.RunCommandLogLevelOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<Output>(new[] { "-o", "--output" }, 
            getDefaultValue: () => Output.Json,
            description: Resources.CommandOutputOption);

        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(logLevelOption);
        AddOption(addressOption);
        AddOption(outputOption);
    }
}