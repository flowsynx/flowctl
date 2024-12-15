using System.CommandLine;

namespace FlowCtl.Commands.Config.Add;

internal class AddConfigCommand : BaseCommand<AddConfigCommandOptions, AddConfigCommandOptionsHandler>
{
    public AddConfigCommand() : base("add", Resources.AddConfigCommandDescription)
    {
        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: Resources.CommandFieldOption) { IsRequired = true };

        var dataFileOption = new Option<string?>(new[] { "-f", "--data-file" },
            description: Resources.InvokeCommandDataFileOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(dataOption);
        AddOption(dataFileOption);
        AddOption(addressOption);
    }
}