namespace FlowSynx.Cli.Commands.Storage;

internal class StorageCommand : BaseCommand
{
    public StorageCommand() : base("storage", "Storage commands")
    {
        AddCommand(new AboutCommand());
    }
}