using System.CommandLine;

namespace FlowSynx.Cli.Commands.Storage.Write;

internal class WriteCommand : BaseCommand<WriteCommandOptions, WriteCommandOptionsHandler>
{
    public WriteCommand() : base("write", Resources.WriteCommandDescription)
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: Resources.WriteCommandPathOption) { IsRequired = true };

        var dataOption = new Option<string?>(new[] { "-d", "--data" },
            description: Resources.WriteCommandDataOption);

        var overWriteOption = new Option<bool?>(new[] { "-o", "--overwrite" }, 
            getDefaultValue: () => false,
            description: Resources.WriteCommandOverwriteOption);

        var fileToUploadOption = new Option<string?>(new[] { "-f", "--file-to-upload" },
            description: Resources.WriteCommandFileToUploadOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.WriteCommandFileToUploadOption);

        AddOption(pathOption);
        AddOption(dataOption);
        AddOption(overWriteOption);
        AddOption(fileToUploadOption);
        AddOption(addressOption);
    }
}