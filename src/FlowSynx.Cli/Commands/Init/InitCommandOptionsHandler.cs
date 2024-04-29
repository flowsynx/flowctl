using EnsureThat;
using FlowSynx.Cli.Common;
using FlowSynx.Cli.Services.Abstracts;
using FlowSynx.IO;

namespace FlowSynx.Cli.Commands.Init;

internal class InitCommandOptionsHandler : ICommandOptionsHandler<InitCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IVersionHandler _versionHandler;
    private readonly IGitHub _gitHub;
    private readonly IExtractor _extractor;
    private readonly ILocation _location;

    public InitCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IVersionHandler versionHandler, IGitHub gitHub,
        IExtractor extractor, ILocation location)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(versionHandler, nameof(versionHandler));
        EnsureArg.IsNotNull(gitHub, nameof(gitHub));
        EnsureArg.IsNotNull(extractor, nameof(extractor));
        EnsureArg.IsNotNull(location, nameof(location));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _versionHandler = versionHandler;
        _gitHub = gitHub;
        _extractor = extractor;
        _location = location;
    }
    
    public async Task<int> HandleAsync(InitCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(InitCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _outputFormatter.Write("Beginning Initialize...");

            var flowSynxPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "engine");
            var flowSynxBinaryFile = _location.LookupFlowSynxBinaryFilePath(flowSynxPath);

            var dashboardPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "dashboard");
            var dashboardBinaryFile = _location.LookupDashboardBinaryFilePath(dashboardPath);

            if (File.Exists(flowSynxBinaryFile) && File.Exists(dashboardBinaryFile))
            {
                _outputFormatter.Write("The FlowSynx engine is already initialized.");
                _outputFormatter.Write("You can use command 'Synx update' to check and update the FlowSynx.");
                return;
            }
            Directory.CreateDirectory(flowSynxPath);
            Directory.CreateDirectory(dashboardPath);

            var initFlowSynx = await InitFlowSynx(options.FlowSynxVersion, cancellationToken);
            if (!initFlowSynx)
                return;

            var initDashboard = await InitDashboard(options.DashboardVersion, cancellationToken);
            if (!initDashboard)
                return;

            _outputFormatter.Write("Starting to change the execution mode of FlowSynx.");
            PathHelper.MakeExecutable(flowSynxBinaryFile);

            _outputFormatter.Write("Starting to change the execution mode of Dashboard.");
            PathHelper.MakeExecutable(flowSynxBinaryFile);

            _outputFormatter.Write("FlowSynx engine is downloaded and installed successfully.");
        }
        catch (Exception e)
        {
            _outputFormatter.WriteError(e.Message);
        }
    }

    private async Task<bool> InitFlowSynx(string? version, CancellationToken cancellationToken)
    {
        var flowSynxVersion = await _gitHub.GetLatestVersion(_gitHub.FlowSynxRepository, cancellationToken);
        if (!string.IsNullOrEmpty(version))
            flowSynxVersion = version;

        flowSynxVersion = _versionHandler.Normalize(flowSynxVersion);

        _outputFormatter.Write("Start download FlowSynx binary");
        var flowSynxDownloadPath = await _gitHub.DownloadAsset(_gitHub.FlowSynxRepository, flowSynxVersion, _gitHub.FlowSynxArchiveFileName, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write("Start validating FlowSynx binary");
        var isFlowSynxValid = await _gitHub.ValidateDownloadedAsset(flowSynxDownloadPath, _gitHub.FlowSynxRepository, flowSynxVersion, _gitHub.FlowSynxArchiveHashFileName, cancellationToken);

        if (isFlowSynxValid)
        {
            _outputFormatter.Write("Starting extract FlowSynx binary");
            ExtractAsset(flowSynxDownloadPath, "engine", cancellationToken);
            return true;
        }

        _outputFormatter.Write("Validating download - Fail!");
        _outputFormatter.Write("The downloaded data may has been corrupted!");
        return false;
    }

    private async Task<bool> InitDashboard(string? version, CancellationToken cancellationToken)
    {
        var dashboardVersion = await _gitHub.GetLatestVersion(_gitHub.DashboardRepository, cancellationToken);
        if (!string.IsNullOrEmpty(version))
            dashboardVersion = version;

        dashboardVersion = _versionHandler.Normalize(dashboardVersion);

        _outputFormatter.Write("Start download Dashboard binary");
        var dashboardDownloadPath = await _gitHub.DownloadAsset(_gitHub.DashboardRepository, dashboardVersion, _gitHub.DashboardArchiveFileName, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write("Start validating Dashboard binary");
        var isFlowSynxValid = await _gitHub.ValidateDownloadedAsset(dashboardDownloadPath, _gitHub.DashboardRepository, dashboardVersion, _gitHub.DashboardArchiveHashFileName, cancellationToken);

        if (isFlowSynxValid)
        {
            _outputFormatter.Write("Starting extract Dashboard binary");
            ExtractAsset(dashboardDownloadPath, "dashboard", cancellationToken);
            return true;
        }

        _outputFormatter.Write("Validating download - Fail!");
        _outputFormatter.Write("The downloaded data may has been corrupted!");
        return false;
    }
    
    private void ExtractAsset(string sourcePath, string destinationPathName, CancellationToken cancellationToken)
    {
        var extractTarget = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, destinationPathName, "downloadedFiles");
        var destinationPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, destinationPathName);

        Directory.CreateDirectory(extractTarget);
        _extractor.ExtractFile(sourcePath, extractTarget);
        Directory.CreateDirectory(destinationPath);
        PathHelper.CopyFilesRecursively(extractTarget, destinationPath, cancellationToken);
        Directory.Delete(extractTarget, true);
    }
}