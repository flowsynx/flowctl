using System.CommandLine;

namespace FlowCtl.Commands.Init;

internal class InitCommand : BaseCommand<InitCommandOptions, InitCommandOptionsHandler>
{
    public InitCommand() : base("init", Resources.Commands_Init_Description)
    {
        var flowSynxVersionOption = new Option<string?>("--flowsynx-version",
            description: Resources.Commands_Init_FlowSynxVersionOption);

        var consoleVersionOption = new Option<string?>("--console-version",
            description: Resources.Commands_Init_FlowSynxVersionOption);

        var dockerOption = new Option<bool>("--docker",
            getDefaultValue: () => false,
            description: Resources.Commands_Init_DockerOption);

        var containerNameOption = new Option<string?>("--container-name",
            getDefaultValue: () => "flowsynx-engine",
            description: Resources.Commands_Init_ContainerNameOption);

        var portOption = new Option<int>("--port",
            getDefaultValue: () => 6262,
            description: Resources.Commands_Init_PortOption);

        var mountOption = new Option<string?>("--mount",
            description: Resources.Commands_Init_MountOption);

        var containerPathOption = new Option<string?>("--container-path",
            getDefaultValue: () => "/app/data",
            description: Resources.Commands_Init_ContainerPathOption);

        AddOption(flowSynxVersionOption);
        AddOption(consoleVersionOption);
        AddOption(dockerOption);
        AddOption(containerNameOption);
        AddOption(portOption);
        AddOption(mountOption);
        AddOption(containerPathOption);
    }
}
