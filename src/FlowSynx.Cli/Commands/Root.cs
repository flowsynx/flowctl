using System.CommandLine;

namespace FlowSynx.Cli.Commands;

public class Root : RootCommand
{
    public Root() : base("A system for managing and synchronizing data between different repositories and storage, including cloud, local, and etc.")
    {

    }
}