using System.CommandLine;

namespace FlowSynx.Cli.Commands.Config.Add;

internal class AddConfigCommand : BaseCommand<AddConfigCommandOptions, AddConfigCommandOptionsHandler>
{
    public AddConfigCommand() : base("add", "Add configuration section")
    {
        var nameOption = new Option<string>(new[] { "-n", "--name" }, "The unique configuration section name") { IsRequired = true };
        var typeOption = new Option<string>(new[] { "-t", "--type" }, "The type of plugin") { IsRequired = true };
        var specificationsOption = new Option<string>(new[] { "-s", "--spec" }, "The specifications regarding configuration section. They should be passed in pairs of key value");

        AddOption(nameOption);
        AddOption(typeOption);
        AddOption(specificationsOption);
    }
}