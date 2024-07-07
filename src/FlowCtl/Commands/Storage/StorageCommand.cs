using FlowCtl.Commands.Storage.About;
using FlowCtl.Commands.Storage.Check;
using FlowCtl.Commands.Storage.Compress;
using FlowCtl.Commands.Storage.Copy;
using FlowCtl.Commands.Storage.Delete;
using FlowCtl.Commands.Storage.DeleteFile;
using FlowCtl.Commands.Storage.ExistFile;
using FlowCtl.Commands.Storage.List;
using FlowCtl.Commands.Storage.MakeDriectory;
using FlowCtl.Commands.Storage.Move;
using FlowCtl.Commands.Storage.PurgeDirectory;
using FlowCtl.Commands.Storage.Read;
using FlowCtl.Commands.Storage.Size;
using FlowCtl.Commands.Storage.Write;

namespace FlowCtl.Commands.Storage;

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