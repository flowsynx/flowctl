using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cli.Commands.Storage;

internal class StorageCommand : BaseCommand
{
    public StorageCommand() : base("storage", "Storage commands")
    {
        AddCommand(new AboutCommand());
    }
}