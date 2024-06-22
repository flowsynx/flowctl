using System.CommandLine;

namespace FlowSynx.Cli.Commands.Config.Add;

internal class AddConfigCommand : BaseCommand<AddConfigCommandOptions, AddConfigCommandOptionsHandler>
{
    public AddConfigCommand() : base("add", Resources.AddConfigCommandDescription)
    {
        var nameOption = new Option<string>(new[] { "-n", "--name" },
            description: Resources.AddConfigCommandNameOption) { IsRequired = true };

        var typeOption = new Option<string>(new[] { "-t", "--type" },
            description: Resources.AddConfigCommandTypeOption) { IsRequired = true };

        var specificationsOption = new Option<string>(new[] { "-s", "--spec" },
            description: Resources.AddConfigCommandSpecificationOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(nameOption);
        AddOption(typeOption);
        AddOption(specificationsOption);
        AddOption(addressOption);
    }
}