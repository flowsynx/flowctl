using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.DeleteFile;

internal class DeleteFileCommand : BaseCommand<DeleteFileCommandOptions, DeleteFileCommandOptionsHandler>
{
    public DeleteFileCommand() : base("deletefile", Resources.DeleteFileCommandDescription)
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: Resources.DeleteFileCommandPathOption) { IsRequired = true };

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(pathOption);
        AddOption(addressOption);
    }
}