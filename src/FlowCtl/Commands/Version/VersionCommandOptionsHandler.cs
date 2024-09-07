using EnsureThat;
using FlowCtl.Services.Abstracts;

namespace FlowCtl.Commands.Version;

internal class VersionCommandOptionsHandler : ICommandOptionsHandler<VersionCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IVersionHandler _versionHandler;
    private readonly ILocation _location;

    public VersionCommandOptionsHandler(IOutputFormatter outputFormatter,
         IVersionHandler versionHandler, ILocation location)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(versionHandler, nameof(versionHandler));
        EnsureArg.IsNotNull(location, nameof(location));
        _outputFormatter = outputFormatter;
        _versionHandler = versionHandler;
        _location = location;
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
                FlowCtl = _versionHandler.Version,
                FlowSynx = GetFlowSynxVersion()
            };

            _outputFormatter.Write(fullVersion, options.Output);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
            return Task.CompletedTask;
        }
    }

    private string GetFlowSynxVersion()
    {
        var flowSynxBinaryFile = _location.LookupFlowSynxBinaryFilePath(Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "engine"));
        var flowSynxVersion = _versionHandler.GetApplicationVersion(flowSynxBinaryFile);
        return string.IsNullOrEmpty(flowSynxVersion) ? Resources.VersionCommandNotInitialized : flowSynxVersion;
    }
}