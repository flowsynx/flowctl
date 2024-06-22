using System.CommandLine;

namespace FlowSynx.Cli.Commands;

public class Root : RootCommand
{
    public Root() : base(Resources.RootCommandDescription)
    {

    }
}