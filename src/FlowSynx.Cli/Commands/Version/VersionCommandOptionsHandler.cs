using EnsureThat;
using FlowCtl.Common;
using FlowCtl.Services.Abstracts;

namespace FlowCtl.Commands.Version;

internal class VersionCommandOptionsHandler : ICommandOptionsHandler<VersionCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IVersionHandler _versionHandler;
    private readonly ILocation _location;

    public VersionCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
         IVersionHandler versionHandler, ILocation location)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(versionHandler, nameof(versionHandler));
        EnsureArg.IsNotNull(location, nameof(location));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _versionHandler = versionHandler;
        _location = location;
    }

    public async Task<int> HandleAsync(VersionCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(() => Execute(options));
        return 0;
    }

    private Task Execute(VersionCommandOptions options)
    {
        try
        {
            var fullVersion = new VersionResponse
            {
                Cli = _versionHandler.Version,
                FlowSynx = GetFlowSynxVersion(),
                Dashboard = GetDashboardVersion()
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

    private string GetDashboardVersion()
    {
        var dashboardBinaryFile = _location.LookupDashboardBinaryFilePath(Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "dashboard"));
        var dashboardVersion = _versionHandler.GetApplicationVersion(dashboardBinaryFile);
        return string.IsNullOrEmpty(dashboardVersion) ? Resources.VersionCommandNotInitialized : dashboardVersion;
    }
}