using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Services.Version;

namespace FlowCtl.Commands.Version;

internal class VersionCommandOptionsHandler : ICommandOptionsHandler<VersionCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;
    private readonly IVersion _version;

    public VersionCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        ILocation location, IVersion version)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _location = location ?? throw new ArgumentNullException(nameof(location));
        _version = version ?? throw new ArgumentNullException(nameof(version));
    }

    public async Task<int> HandleAsync(VersionCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options);
        return 0;
    }

    private Task Execute(VersionCommandOptions options)
    {
        try
        {
            var fullVersion = new VersionResponse
            {
                FlowCtl = _version.FlowCtlVersion,
                FlowSynx = GetFlowSynxVersion()
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

    private string GetFlowSynxVersion()
    {
        var flowSynxBinaryFile = _location.LookupFlowSynxBinaryFilePath(Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "engine"));
        var flowSynxVersion = _version.GetVersionFromPath(flowSynxBinaryFile);
        return string.IsNullOrEmpty(flowSynxVersion) ? Resources.VersionCommandNotInitialized : flowSynxVersion;
    }
}