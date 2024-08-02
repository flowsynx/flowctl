namespace FlowCtl.Commands.Dashboard.Version;

internal class DashboardVersionCommandOptions : ICommandOptions
{
    public Output Output { get; set; } = Output.Json;
}