using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.ExistFile;

internal class ExistFileCommand : BaseCommand<ExistFileCommandOptions, ExistFileCommandOptionsHandler>
{
    public ExistFileCommand() : base("exist", Resources.ExistFileCommandDescription)
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, 
            description: Resources.ExistFileCommandPathOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(pathOption);
        AddOption(addressOption);
    }
}