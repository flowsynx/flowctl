using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Logs;

internal class LogsCommand : BaseCommand<LogsCommandOptions, LogsCommandOptionsHandler>
{
    public LogsCommand() : base("logs", Resources.LogsCommandDescription)
    {
        var levelOption = new Option<string?>(new[] { "-l", "--level" },
            description: Resources.InvokeCommandDataOption);

        var fromDateOption = new Option<string?>(new[] { "-f", "--from-date" },
            description: Resources.InvokeCommandDataFileOption);

        var toDateOption = new Option<string?>(new[] { "-t", "--to-date" },
            description: Resources.ReadCommandSaveToOption);

        var messageDateOption = new Option<string?>(new[] { "-m", "--message" },
            description: Resources.ReadCommandSaveToOption);

        var exportPathOption = new Option<string?>(new[] { "-e", "--export-to" },
            description: Resources.ReadCommandSaveToOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<OutputType>(new[] { "-o", "--output" },
            getDefaultValue: () => OutputType.Json,
            description: Resources.CommandOutputOption);

        AddOption(levelOption);
        AddOption(fromDateOption);
        AddOption(toDateOption);
        AddOption(messageDateOption);
        AddOption(exportPathOption);
        AddOption(addressOption);
        AddOption(outputOption);
    }
}