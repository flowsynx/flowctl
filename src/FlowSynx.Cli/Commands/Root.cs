using System.CommandLine;

namespace FlowSynx.Cli.Commands;

public class Root : RootCommand
{
    public Root() : base("An system for managing and synchronizing data between different repositories, including cloud storage, local, and etc.")
    {

    }
}