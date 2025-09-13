using FlowCtl.Commands.Console.Stop;
using FlowCtl.Commands.Console.Version;
using System.CommandLine;

namespace FlowCtl.Commands.Console;

internal class ConsoleCommand : BaseCommand<ConsoleCommandOptions, ConsoleCommandOptionsHandler>
{
    public ConsoleCommand() : base("console", Resources.Command_Console_Description)
    {
        var backgroundOption = new Option<bool>(new[] { "-b", "--background" },
            getDefaultValue: () => false,
            description: Resources.Command_Console_BackgroundOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.Commands_FlowSynxAddress);

        AddOption(backgroundOption);
        AddOption(addressOption);

        AddCommand(new CosnoleStopCommand());
        AddCommand(new ConsoleVersionCommand());
    }
}