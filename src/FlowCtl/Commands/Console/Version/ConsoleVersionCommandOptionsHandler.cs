using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Services.Version;

namespace FlowCtl.Commands.Console.Version;

internal class ConsoleVersionCommandOptionsHandler : ICommandOptionsHandler<ConsoleVersionCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;
    private readonly IVersion _version;

    public ConsoleVersionCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        ILocation location, IVersion version)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _location = location ?? throw new ArgumentNullException(nameof(location));
        _version = version ?? throw new ArgumentNullException(nameof(version));
    }

    public Task<int> HandleAsync(ConsoleVersionCommandOptions options, CancellationToken cancellationToken)
    {
        Execute(options);
        return Task.FromResult(0);
    }

    private Task Execute(ConsoleVersionCommandOptions options)
    {
        try
        {
            var fullVersion = new
            {
                Version = GetConsoleVersion()
            };

            _flowCtlLogger.Write(fullVersion, options.Output);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
            return Task.CompletedTask;
        }
    }

    private string GetConsoleVersion()
    {
        var consoleBinaryFile = _location.LookupConsoleBinaryFilePath(Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "console"));
        var consoleVersion = _version.GetVersionFromPath(consoleBinaryFile);
        return string.IsNullOrEmpty(consoleVersion) ? Resources.Commands_Version_NotInitialized : consoleVersion;
    }
}