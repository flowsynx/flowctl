using FlowSynx.Cli.Commands.Storage.About;
using FlowSynx.Cli.Commands.Storage.Check;
using FlowSynx.Cli.Commands.Storage.Compress;
using FlowSynx.Cli.Commands.Storage.Copy;
using FlowSynx.Cli.Commands.Storage.Delete;
using FlowSynx.Cli.Commands.Storage.DeleteFile;
using FlowSynx.Cli.Commands.Storage.ExistFile;
using FlowSynx.Cli.Commands.Storage.List;
using FlowSynx.Cli.Commands.Storage.MakeDriectory;
using FlowSynx.Cli.Commands.Storage.Move;
using FlowSynx.Cli.Commands.Storage.PurgeDirectory;
using FlowSynx.Cli.Commands.Storage.Read;
using FlowSynx.Cli.Commands.Storage.Size;
using FlowSynx.Cli.Commands.Storage.Write;

namespace FlowSynx.Cli.Commands.Storage;

internal class StorageCommand : BaseCommand
{
    public StorageCommand() : base("storage", Resources.StorageCommandDescription)
    {
        AddCommand(new AboutCommand());
        AddCommand(new CheckCommand());
        AddCommand(new CompressCommand());
        AddCommand(new CopyCommand());
        AddCommand(new DeleteCommand());
        AddCommand(new DeleteFileCommand());
        AddCommand(new ExistFileCommand());
        AddCommand(new ListCommand());
        AddCommand(new MakeDirectoryCommand());
        AddCommand(new MoveCommand());
        AddCommand(new PurgeDirectoryCommand());
        AddCommand(new ReadCommand());
        AddCommand(new SizeCommand());
        AddCommand(new WriteCommand());
    }
}