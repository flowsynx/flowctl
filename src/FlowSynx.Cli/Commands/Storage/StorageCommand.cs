namespace FlowSynx.Cli.Commands.Storage;

internal class StorageCommand : BaseCommand
{
    public StorageCommand() : base("storage", "Execute supportable commands on different storage supported by FlowSynx system")
    {
        AddCommand(new AboutCommand());
        AddCommand(new CopyCommand());
        AddCommand(new DeleteCommand());
        AddCommand(new DeleteFileCommand());
        AddCommand(new ListCommand());
        AddCommand(new MakeDirectoryCommand());
        AddCommand(new MoveCommand());
        AddCommand(new PurgeDirectoryCommand());
        AddCommand(new ReadCommand());
        AddCommand(new SizeCommand());
        AddCommand(new WriteCommand());
    }
}