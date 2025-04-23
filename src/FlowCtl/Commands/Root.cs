using System.CommandLine;

namespace FlowCtl.Commands;

public class Root : RootCommand
{
    public Root() : base(Resources.Commands_RootDescription)
    {

    }
}