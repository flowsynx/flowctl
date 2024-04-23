using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.DeleteFile;

internal class DeleteFileCommand : BaseCommand<DeleteFileCommandOptions, DeleteFileCommandOptionsHandler>
{
    public DeleteFileCommand() : base("deletefile", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, "The path to get about") { IsRequired = true };
        var urlOption = new Option<string?>(new[] { "-u", "--url" }, "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(pathOption);
        AddOption(urlOption);
    }
}