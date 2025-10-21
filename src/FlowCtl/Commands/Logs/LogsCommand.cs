using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Logs;

internal class LogsCommand : BaseCommand<LogsCommandOptions, LogsCommandOptionsHandler>
{
    public LogsCommand() : base("logs", Resources.Commands_Logs_Description)
    {
        var levelOption = new Option<string?>(new[] { "-l", "--level" },
            description: Resources.Commands_Logs_LogLevelOption);

        var fromDateOption = new Option<string?>(new[] { "-f", "--from-date" },
            description: Resources.Commands_Logs_FromDateOption);

        var toDateOption = new Option<string?>(new[] { "-t", "--to-date" },
            description: Resources.Commands_Logs_EndDateOption);

        var messageDateOption = new Option<string?>(new[] { "-m", "--message" },
            description: Resources.Commands_Logs_MessageOption);

        var exportPathOption = new Option<string?>(new[] { "-e", "--export-to" },
            description: Resources.Commands_Logs_ExportToOption);

        var pageOption = new Option<int?>(new[] { "-p", "--page" },
            description: Resources.Commands_FlowSynxPage);

        var pageSizeOption = new Option<int?>(new[] { "-s", "--page-size" },
            description: Resources.Commands_FlowSynxPageSize);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        var outputOption = new Option<OutputType>(new[] { "-o", "--output" },
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);

        AddOption(levelOption);
        AddOption(fromDateOption);
        AddOption(toDateOption);
        AddOption(messageDateOption);
        AddOption(exportPathOption);
        AddOption(pageOption);
        AddOption(pageSizeOption);
        AddOption(addressOption);
        AddOption(outputOption);
    }
}