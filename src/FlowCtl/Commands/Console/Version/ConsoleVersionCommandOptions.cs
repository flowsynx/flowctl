using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Console.Version;

internal class ConsoleVersionCommandOptions : ICommandOptions
{
    public OutputType Output { get; set; } = OutputType.Json;
}
