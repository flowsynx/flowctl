using FlowCtl.Core.Services.Logger;
using System.CommandLine;

namespace FlowCtl.Commands.Console.Version;

internal class ConsoleVersionCommand : BaseCommand<ConsoleVersionCommandOptions, ConsoleVersionCommandOptionsHandler>
{
    public ConsoleVersionCommand() : base("version", Resources.Command_ConsoleVersion_Description)
    {
        var outputOption = new Option<OutputType>(new[] { "-o", "--output" },
            getDefaultValue: () => OutputType.Json,
            description: Resources.Commands_Output_Format);

        AddOption(outputOption);
    }
}