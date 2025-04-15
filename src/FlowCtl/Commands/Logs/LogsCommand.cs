using FlowCtl.Core.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Logs;

internal class LogsCommand : BaseCommand<LogsCommandOptions, LogsCommandOptionsHandler>
{
    public LogsCommand() : base("logs", Resources.LogsCommandDescription)
    {
        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: Resources.InvokeCommandDataOption);

        var dataFileOption = new Option<string?>(new[] { "-f", "--data-file" },
            description: Resources.InvokeCommandDataFileOption);

        var exportPathOption = new Option<string?>(new[] { "-e", "--export-to" },
            description: Resources.ReadCommandSaveToOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        var outputOption = new Option<OutputType>(new[] { "-o", "--output" },
            getDefaultValue: () => OutputType.Json,
            description: Resources.CommandOutputOption);

        AddOption(dataOption);
        AddOption(dataFileOption);
        AddOption(exportPathOption);
        AddOption(addressOption);
        AddOption(outputOption);
    }
}