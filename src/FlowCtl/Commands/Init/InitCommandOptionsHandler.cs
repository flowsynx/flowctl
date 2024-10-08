﻿using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.IO;

namespace FlowCtl.Commands.Init;

internal class InitCommandOptionsHandler : ICommandOptionsHandler<InitCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IVersionHandler _versionHandler;
    private readonly IGitHub _gitHub;
    private readonly IExtractor _extractor;
    private readonly ILocation _location;

    public InitCommandOptionsHandler(IOutputFormatter outputFormatter,
        IVersionHandler versionHandler, IGitHub gitHub,
        IExtractor extractor, ILocation location)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(versionHandler, nameof(versionHandler));
        EnsureArg.IsNotNull(gitHub, nameof(gitHub));
        EnsureArg.IsNotNull(extractor, nameof(extractor));
        EnsureArg.IsNotNull(location, nameof(location));
        _outputFormatter = outputFormatter;
        _versionHandler = versionHandler;
        _gitHub = gitHub;
        _extractor = extractor;
        _location = location;
    }
    
    public async Task<int> HandleAsync(InitCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(InitCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _outputFormatter.Write(Resources.InitCommandBeginningInitialize);

            var flowSynxPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "engine");
            var flowSynxBinaryFile = _location.LookupFlowSynxBinaryFilePath(flowSynxPath);

            var dashboardPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "dashboard");
            var dashboardBinaryFile = _location.LookupDashboardBinaryFilePath(dashboardPath);

            if (File.Exists(flowSynxBinaryFile) && File.Exists(dashboardBinaryFile))
            {
                _outputFormatter.Write(Resources.TheFlowSynxEngineIsAlreadyInitialized);
                _outputFormatter.Write(Resources.UseUpdateCommandToUpdateFlowSynxAndDashboard);
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

            _outputFormatter.Write(Resources.StartChangeFlowSynxExecutionMode);
            PathHelper.MakeExecutable(flowSynxBinaryFile);

            _outputFormatter.Write(Resources.StartChangeDashboardExecutionMode);
            PathHelper.MakeExecutable(flowSynxBinaryFile);

            _outputFormatter.Write(string.Format(Resources.FlowSynxEngineDownloadedAndInstalledSuccessfully, _location.DefaultFlowSynxBinaryDirectoryName));
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

        _outputFormatter.Write(Resources.StartDownloadFlowSynxBinary);
        var flowSynxDownloadPath = await _gitHub.DownloadAsset(_gitHub.FlowSynxRepository, flowSynxVersion, _gitHub.FlowSynxArchiveFileName, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write(Resources.StartValidatingFlowSynxBinary);
        var isFlowSynxValid = await _gitHub.ValidateDownloadedAsset(flowSynxDownloadPath, _gitHub.FlowSynxRepository, flowSynxVersion, _gitHub.FlowSynxArchiveHashFileName, cancellationToken);

        if (!isFlowSynxValid)
        {
            _outputFormatter.Write(Resources.ValidatingDownloadFail);
            _outputFormatter.Write(Resources.TheDownloadedDataMayHasBeenCorrupted);
            return false;
        }

        _outputFormatter.Write(Resources.StartingExtractFlowSynxBinary);
        ExtractAsset(flowSynxDownloadPath, "engine", cancellationToken);
        return true;
    }

    private async Task<bool> InitDashboard(string? version, CancellationToken cancellationToken)
    {
        var dashboardVersion = await _gitHub.GetLatestVersion(_gitHub.DashboardRepository, cancellationToken);
        if (!string.IsNullOrEmpty(version))
            dashboardVersion = version;

        dashboardVersion = _versionHandler.Normalize(dashboardVersion);

        _outputFormatter.Write(Resources.StartDownloadDashboardBinary);
        var dashboardDownloadPath = await _gitHub.DownloadAsset(_gitHub.DashboardRepository, dashboardVersion, _gitHub.DashboardArchiveFileName, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write(Resources.StartValidatingDashboardBinary);
        var isDashboardValid = await _gitHub.ValidateDownloadedAsset(dashboardDownloadPath, _gitHub.DashboardRepository, dashboardVersion, _gitHub.DashboardArchiveHashFileName, cancellationToken);

        if (!isDashboardValid)
        {
            _outputFormatter.Write(Resources.ValidatingDownloadFail);
            _outputFormatter.Write(Resources.TheDownloadedDataMayHasBeenCorrupted);
            return false;
        }

        _outputFormatter.Write(Resources.StartingExtractDashboardBinary);
        ExtractAsset(dashboardDownloadPath, "dashboard", cancellationToken);
        return true;
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