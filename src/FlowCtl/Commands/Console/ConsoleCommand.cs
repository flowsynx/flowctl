using FlowCtl.Commands.Console.Version;
using System.CommandLine;

namespace FlowCtl.Commands.Console;

internal class ConsoleCommand : BaseCommand<ConsoleCommandOptions, ConsoleCommandOptionsHandler>
{
    public ConsoleCommand() : base("console", Resources.Command_Console_Description)
    {
        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(addressOption);
        AddCommand(new ConsoleVersionCommand());
    }
}