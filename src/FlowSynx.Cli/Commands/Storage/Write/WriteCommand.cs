using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Write;

internal class WriteCommand : BaseCommand<WriteCommandOptions, WriteCommandOptionsHandler>
{
    public WriteCommand() : base("write", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>("--path", "The path to get about") { IsRequired = true };
        var dataOption = new Option<string?>("--data", "The path to get about");
        var overWriteOption = new Option<bool?>("--overwrite", getDefaultValue: () => false, "The path to get about");
        var fileToUploadOption = new Option<string?>("--file-to-upload", "The path to get about");

        AddOption(pathOption);
        AddOption(dataOption);
        AddOption(overWriteOption);
        AddOption(fileToUploadOption);
    }
}