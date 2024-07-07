using System.CommandLine;

namespace FlowCtl.Commands.Storage.Read;

internal class ReadCommand : BaseCommand<ReadCommandOptions, ReadCommandOptionsHandler>
{
    public ReadCommand() : base("read", Resources.ReadCommandDescription)
    {
        var pathOption = new Option<string>(new[] { "-p", "--path" },
            description: Resources.ReadCommandPathOption) { IsRequired = true };

        var hashOption = new Option<bool?>(new[] { "+h", "--hashing" }, 
            getDefaultValue: () => false,
            description: Resources.ReadCommandHashingOption);

        var savePathOption = new Option<string>(new[] { "-s", "--save-to" },
            description: Resources.ReadCommandSaveToOption) { IsRequired = true };

        var overWriteOption = new Option<bool?>(new[] { "-o", "--overwrite" }, 
            getDefaultValue: () => false,
            description: Resources.ReadCommandOverwriteOption);

        var addressOption = new Option<string?>(new[] { "-a", "--address" },
            description: Resources.CommandAddressOption);

        AddOption(pathOption);
        AddOption(hashOption);
        AddOption(savePathOption);
        AddOption(overWriteOption);
        AddOption(addressOption);
    }
}