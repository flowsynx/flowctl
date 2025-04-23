using FlowCtl.Core.Services.Extractor;
using FlowCtl.Core.Services.Github;
using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Core.Services.ProcessHost;
using FlowCtl.Services.Version;

namespace FlowCtl.Commands.Update;

internal class UpdateCommandOptionsHandler : ICommandOptionsHandler<UpdateCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IGitHubReleaseManager _gitHubReleaseManager;
    private readonly IArchiveExtractor _archiveExtractor;
    private readonly IProcessHandler _processHandler;
    private readonly IVersion _version;
    private readonly ILocation _location;

    public UpdateCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IGitHubReleaseManager gitHubReleaseManager, IArchiveExtractor archiveExtractor, 
        IProcessHandler processHandler, IVersion version, ILocation location)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _gitHubReleaseManager = gitHubReleaseManager ?? throw new ArgumentNullException(nameof(gitHubReleaseManager));
        _archiveExtractor = archiveExtractor ?? throw new ArgumentNullException(nameof(archiveExtractor));
        _processHandler = processHandler ?? throw new ArgumentNullException(nameof(processHandler));
        _version = version ?? throw new ArgumentNullException(nameof(version));
        _location = location ?? throw new ArgumentNullException(nameof(location));
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
            _flowCtlLogger.Write(Resources.Commands_Update_CheckingForFlowSynxSystemUpdates);
            await UpdateFlowSynx(options, cancellationToken);
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }

    private async Task UpdateFlowSynx(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        var latestVersion = await _gitHubReleaseManager.GetLatestVersion(GitHubSettings.Organization, GitHubSettings.FlowSynxRepository);
        var binaryFile = _location.LookupFlowSynxBinaryFilePath(Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "engine"));
        var checkVersionResult = CheckVersion(binaryFile, options.FlowSynxVersion, latestVersion);
        if (checkVersionResult.IsUpdateAvailable)
        {
            var isProcessStopped = _processHandler.IsStopped("flowsynx", ".", options.Force);
            if (!isProcessStopped)
            {
                _flowCtlLogger.Write(Resources.Commands_Update_FlowSynxSystemIsRunning);
                return;
            }

            var downloadAndValidate = await DownloadAndValidateAndExtractFlowSynx(checkVersionResult.Version, cancellationToken);
            if (!downloadAndValidate)
                return;
        }
        else
        {
            _flowCtlLogger.Write(Resources.Commands_Update_FlowSynxSystemIsUpToDate);
        }
    }

    private CheckVersionResult CheckVersion(string binaryPath, string? version, string latestVersion)
    {
        var currentVersion = _version.GetVersionFromPath(binaryPath);

        if (!string.IsNullOrEmpty(version))
            latestVersion = version;

        latestVersion = NormalizeVersion(latestVersion);

        return new CheckVersionResult
        {
            IsUpdateAvailable = CheckVersions(latestVersion, currentVersion),
            Version = latestVersion
        };
    }

    private async Task<bool> DownloadAndValidateAndExtractFlowSynx(string version, CancellationToken cancellationToken)
    {
        _flowCtlLogger.Write(Resources.Commands_Update_StartDownloadFlowSynxSystemBinary);

        string tempPath = Path.Combine(Path.GetTempPath(), GitHubSettings.FlowSynxArchiveTemporaryFileName);
        var flowSynxDownloadPath = await _gitHubReleaseManager.DownloadAsset(GitHubSettings.Organization,
            GitHubSettings.FlowSynxRepository, GitHubSettings.FlowSynxArchiveFileName, tempPath, version);

        string tempHashPath = Path.Combine(Path.GetTempPath(), GitHubSettings.FlowSynxArchiveTemporaryHashFileName);
        var flowSynxHashDownloadPath = await _gitHubReleaseManager.DownloadAsset(GitHubSettings.Organization,
            GitHubSettings.FlowSynxRepository, GitHubSettings.FlowSynxArchiveHashFileName, tempHashPath, version);

        _flowCtlLogger.Write(Resources.Commands_Update_StartValidatingFlowSynxSystemBinary);
        var isFlowSynxValid = _gitHubReleaseManager.ValidateDownloadedAsset(flowSynxDownloadPath, flowSynxHashDownloadPath);

        if (!isFlowSynxValid)
        {
            _flowCtlLogger.Write(Resources.Commands_Update_ValidatingFlowSynxSystemFailed);
            _flowCtlLogger.Write(Resources.Commands_Update_DownloadedDataCorrupted);
            return false;
        }

        _flowCtlLogger.Write(Resources.Commands_Update_StartExtractingFlowSynxSystemBinary);
        ExtractAsset(flowSynxDownloadPath, "engine", cancellationToken);
        return true;
    }

    private void ExtractAsset(string sourcePath, string destinationPathName, CancellationToken cancellationToken)
    {
        var extractTarget = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, destinationPathName, "downloadedFiles");
        var destinationPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, destinationPathName);

        Directory.CreateDirectory(extractTarget);
        _archiveExtractor.ExtractArchive(sourcePath, extractTarget);
        Directory.CreateDirectory(destinationPath);
        CopyFilesRecursively(extractTarget, destinationPath, cancellationToken);
        Directory.Delete(extractTarget, true);
    }

    private string NormalizeVersion(string? version)
    {
        if (string.IsNullOrEmpty(version))
            return string.Empty;

        return !version.StartsWith("v") ? $"v{version}" : version;
    }

    private bool CheckVersions(string latestVersion, string currentVersion)
    {
        if (string.IsNullOrEmpty(latestVersion) || string.IsNullOrEmpty(currentVersion))
            return false;

        var latestParts = CleanVersion(latestVersion).Split('.');
        var currentParts = CleanVersion(currentVersion).Split('.');

        int maxLength = Math.Max(latestParts.Length, currentParts.Length);

        for (int i = 0; i < maxLength; i++)
        {
            int latestPart = i < latestParts.Length ? int.Parse(latestParts[i]) : 0;
            int currentPart = i < currentParts.Length ? int.Parse(currentParts[i]) : 0;

            if (latestPart > currentPart)
                return true;
            else if (latestPart < currentPart)
                return false;
        }

        return false; // versions are equal
    }

    private string CleanVersion(string version)
    {
        // Remove leading 'v' or 'V' and any suffix like "-beta"
        int dashIndex = version.IndexOf('-');
        if (dashIndex != -1)
            version = version.Substring(0, dashIndex);

        if (version.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            version = version.Substring(1);

        return version;
    }

    private void CopyFilesRecursively(string sourcePath, string targetPath, CancellationToken cancellationToken)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}