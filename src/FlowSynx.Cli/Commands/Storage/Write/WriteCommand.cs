using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Write;

internal class WriteCommand : BaseCommand<WriteCommandOptions, WriteCommandOptionsHandler>
{
    public WriteCommand() : base("write", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" }, "The path to get about") { IsRequired = true };
        var dataOption = new Option<string?>(new[] { "-d", "--data" }, "The path to get about");
        var overWriteOption = new Option<bool?>(new[] { "-o", "--overwrite" }, getDefaultValue: () => false, "The path to get about");
        var fileToUploadOption = new Option<string?>(new[] { "-f", "--file-to-upload" }, "The path to get about");
        var urlOption = new Option<string?>(new[] { "-u", "--url" }, "The address that specify a URL to connect on remote FlowSynx system");

        AddOption(pathOption);
        AddOption(dataOption);
        AddOption(overWriteOption);
        AddOption(fileToUploadOption);
        AddOption(urlOption);
    }
}