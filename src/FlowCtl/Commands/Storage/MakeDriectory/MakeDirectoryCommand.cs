using System.CommandLine;

namespace FlowCtl.Commands.Storage.MakeDriectory;

internal class MakeDirectoryCommand : BaseCommand<MakeDirectoryCommandOptions, MakeDirectoryCommandOptionsHandler>
{
    public MakeDirectoryCommand() : base("mkdir", Resources.MakeDirectoryCommandDescription)
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: Resources.MakeDirectoryCommandPathOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(pathOption);
        AddOption(addressOption);
    }
}