using System.Diagnostics;
using System.Runtime.InteropServices;
using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Environment;
using FlowSynx.IO;

namespace FlowCtl.Commands.Update;

internal class UpdateCommandOptionsHandler : ICommandOptionsHandler<UpdateCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IVersionHandler _versionHandler;
    private readonly ILocation _location;
    private readonly IGitHub _gitHub;
    private readonly IExtractor _extractor;
    private readonly IProcessHandler _processHandler;

    public UpdateCommandOptionsHandler(IOutputFormatter outputFormatter,
        IVersionHandler versionHandler, ILocation location, IGitHub gitHub,
        IExtractor extractor, IProcessHandler processHandler)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(versionHandler, nameof(versionHandler));
        EnsureArg.IsNotNull(location, nameof(location));
        EnsureArg.IsNotNull(gitHub, nameof(gitHub));
        EnsureArg.IsNotNull(extractor, nameof(extractor));
        _outputFormatter = outputFormatter;
        _versionHandler = versionHandler;
        _location = location;
        _gitHub = gitHub;
        _extractor = extractor;
        _processHandler = processHandler;
    }

    public async Task<int> HandleAsync(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _outputFormatter.Write(Resources.UpdateCommandCheckingForFlowSynxUpdates);
            await UpdateFlowSynx(options, cancellationToken);

            _outputFormatter.Write(Resources.UpdateCommandCheckingForDashboardUpdates);
            await UpdateDashboard(options, cancellationToken);

            _outputFormatter.Write(Resources.UpdateCommandCheckingForCliUpdates);
            await UpdateFlowCtl(options, cancellationToken);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }

    private async Task UpdateFlowSynx(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        var latestVersion = await _gitHub.GetLatestVersion(_gitHub.FlowSynxRepository, cancellationToken);
        var binaryFile = _location.LookupFlowSynxBinaryFilePath(Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "engine"));
        var checkVersionResult = CheckVersion(binaryFile, options.FlowSynxVersion, latestVersion);
        if (checkVersionResult.IsUpdateAvailable)
        {
            var isProcessStopped = _processHandler.IsStopped("flowsynx", ".", options.Force);
            if (!isProcessStopped)
            {
                _outputFormatter.Write(Resources.UpdateCommandFlowSynxIsRunning);
                return;
            }

            var downloadAndValidate = await DownloadAndValidateAndExtractFlowSynx(checkVersionResult.Version, cancellationToken);
            if (!downloadAndValidate)
                return;
        }
        else
        {
            _outputFormatter.Write(Resources.UpdateCommandFlowSynxIsUpdated);
        }
    }

    private async Task UpdateDashboard(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        var latestVersion = await _gitHub.GetLatestVersion(_gitHub.DashboardRepository, cancellationToken);
        var binaryFile = _location.LookupDashboardBinaryFilePath(Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "dashboard"));
        var checkVersionResult = CheckVersion(binaryFile, options.DashboardVersion, latestVersion);
        if (checkVersionResult.IsUpdateAvailable)
        {
            var isProcessStopped = _processHandler.IsStopped("dashboard", ".", options.Force);
            if (!isProcessStopped)
            {
                _outputFormatter.Write(Resources.UpdateCommandDashboardIsRunning);
                return;
            }

            var downloadAndValidate = await DownloadAndValidateAndExtractDashboard(checkVersionResult.Version, cancellationToken);
            if (!downloadAndValidate)
                return;
        }
        else
        {
            _outputFormatter.Write(Resources.UpdateCommandDashboardIsUpdated);
        }
    }

    private async Task UpdateFlowCtl(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        var cliLatestVersion = await _gitHub.GetLatestVersion(_gitHub.FlowCtlRepository, cancellationToken);
        cliLatestVersion = _versionHandler.Normalize(cliLatestVersion);

        var cliCurrentVersion = _versionHandler.Version;

        if (_versionHandler.CheckVersions(cliLatestVersion, cliCurrentVersion))
        {
            await DownloadAndValidateAndExtractFlowCtl(cliLatestVersion, cancellationToken);
        }
        else
        {
            _outputFormatter.Write(Resources.UpdateCommandCliIsUpdated);
        }
    }

    private CheckVersionResult CheckVersion(string binaryPath, string? version, string latestVersion)
    {
        var currentVersion = _versionHandler.GetApplicationVersion(binaryPath);

        if (!string.IsNullOrEmpty(version))
            latestVersion = version;

        latestVersion = _versionHandler.Normalize(latestVersion);

        return new CheckVersionResult
        {
            IsUpdateAvailable = _versionHandler.CheckVersions(latestVersion, currentVersion),
            Version = latestVersion
        };
    }

    private async Task<bool> DownloadAndValidateAndExtractFlowSynx(string version, CancellationToken cancellationToken)
    {
        _outputFormatter.Write(Resources.StartDownloadFlowSynxBinary);
        var flowSynxDownloadPath = await _gitHub.DownloadAsset(_gitHub.FlowSynxRepository, version, _gitHub.FlowSynxArchiveFileName, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write(Resources.StartValidatingFlowSynxBinary);
        var isFlowSynxValid = await _gitHub.ValidateDownloadedAsset(flowSynxDownloadPath, _gitHub.FlowSynxRepository, version, _gitHub.FlowSynxArchiveHashFileName, cancellationToken);

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

    private async Task<bool> DownloadAndValidateAndExtractDashboard(string version, CancellationToken cancellationToken)
    {
        _outputFormatter.Write(Resources.StartDownloadDashboardBinary);
        var dashboardDownloadPath = await _gitHub.DownloadAsset(_gitHub.DashboardRepository, version, _gitHub.DashboardArchiveFileName, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write(Resources.StartValidatingDashboardBinary);
        var isDashboardValid = await _gitHub.ValidateDownloadedAsset(dashboardDownloadPath, _gitHub.DashboardRepository, version, _gitHub.DashboardArchiveHashFileName, cancellationToken);

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

    private async Task DownloadAndValidateAndExtractFlowCtl(string version, CancellationToken cancellationToken)
    {
        _outputFormatter.Write(Resources.StartDownloadCliBinary);
        var cliDownloadPath = await _gitHub.DownloadAsset(_gitHub.FlowCtlRepository, version, _gitHub.FlowCtlArchiveFileName, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write(Resources.StartValidatingCliBinary);
        var isCliValid = await _gitHub.ValidateDownloadedAsset(cliDownloadPath, _gitHub.FlowCtlRepository, version, _gitHub.FlowCtlArchiveHashFileName, cancellationToken);

        if (!isCliValid)
        {
            _outputFormatter.Write(Resources.ValidatingDownloadFail);
            _outputFormatter.Write(Resources.TheDownloadedDataMayHasBeenCorrupted);
            return;
        }

        ExtractFlowCtl(cliDownloadPath, cancellationToken);
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

    private async void ExtractFlowCtl(string sourcePath, CancellationToken cancellationToken)
    {
        const string extractTarget = "./downloadedFiles";
        Directory.CreateDirectory(extractTarget);

        _extractor.ExtractFile(sourcePath, extractTarget);
        File.Delete(sourcePath);

        var synxUpdateExeFile = Path.GetFullPath(_location.LookupFlowCtlBinaryFilePath(extractTarget));
        var files = Directory
            .GetFiles(extractTarget, "*.*", SearchOption.AllDirectories)
            .Where(name => !string.Equals(Path.GetFullPath(name), synxUpdateExeFile, StringComparison.InvariantCultureIgnoreCase));

        foreach (var newPath in files)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            File.Copy(newPath, newPath.Replace(extractTarget, "."), true);
        }

        var synxExeFile = Path.GetFullPath(_location.LookupFlowCtlBinaryFilePath(_location.RootLocation));
        await SelfUpdate(synxUpdateExeFile, synxExeFile);
    }
    
    private async Task SelfUpdate(string updateFile, string selfFile)
    {
        await using var stream = File.OpenRead(updateFile);

        var selfFileName = Path.GetFileName(selfFile);
        var directoryName = Path.GetDirectoryName(selfFile);
        var downloadedFilesPath = Path.GetDirectoryName(updateFile);
        if (string.IsNullOrEmpty(directoryName))
            return;

        var selfWithoutExt = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(selfFile));
        stream.WriteTo(selfWithoutExt + _location.GetUpdateFilePath());

        var updateExeFile = selfWithoutExt + _location.GetUpdateFilePath();
        var scriptFile = selfWithoutExt + _location.GetScriptFilePath();

        var updateScript = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            string.Format(Resources.UpdateScript_Bat, selfFileName, updateExeFile, selfFile, updateExeFile, downloadedFilesPath) :
            string.Format(Resources.UpdateScript_Shell, updateExeFile, selfFile, selfFileName, updateExeFile, downloadedFilesPath);

        StreamWriter streamWriter = new(File.Create(scriptFile));

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            streamWriter.NewLine = "\n";

        streamWriter.Write(updateScript);
        streamWriter.Close();

        ProcessStartInfo startInfo = new(scriptFile)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            WorkingDirectory = directoryName
        };

        try
        {
            Process.Start(startInfo);
            _outputFormatter.Write(Resources.UpdateCommandFlowSynxUpdatedSuccessfully);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
            System.Environment.Exit(0);
        }
    }
}