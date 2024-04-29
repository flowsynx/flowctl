using EnsureThat;
using FlowSynx.Cli.Common;
using FlowSynx.Cli.Services;

namespace FlowSynx.Cli.Commands.Version;

internal class VersionCommandOptionsHandler : ICommandOptionsHandler<VersionCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IVersionHandler _versionHandler;

    public VersionCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
         IVersionHandler versionHandler)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(versionHandler, nameof(versionHandler));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _versionHandler = versionHandler;
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
            var cliVersion = _versionHandler.Version;

            if (options.Full is null or false)
            {
                var version = new { Cli = cliVersion };
                _outputFormatter.Write(version, options.Output);
                return Task.CompletedTask;
            }

            var flowSynxBinaryFile = PathHelper.LookupFlowSynxBinaryFilePath(Path.Combine(PathHelper.DefaultFlowSynxBinaryDirectoryName, "engine"));
            var flowSynxVersion = _versionHandler.GetApplicationVersion(flowSynxBinaryFile);

            var dashboardBinaryFile = PathHelper.LookupDashboardBinaryFilePath(Path.Combine(PathHelper.DefaultFlowSynxBinaryDirectoryName, "dashboard"));
            var dashboardVersion = _versionHandler.GetApplicationVersion(dashboardBinaryFile);

            var fullVersion = new VersionResponse
            {
                Cli = cliVersion,
                FlowSynx = string.IsNullOrEmpty(flowSynxVersion) ? "Not initialized" : flowSynxVersion,
                Dashboard = string.IsNullOrEmpty(dashboardVersion) ? "Not initialized" : dashboardVersion
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
}