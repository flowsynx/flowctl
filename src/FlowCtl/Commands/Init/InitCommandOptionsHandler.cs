using FlowCtl.Core.Services.Extractor;
using FlowCtl.Core.Services.Github;
using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;
using System.Runtime.InteropServices;

namespace FlowCtl.Commands.Init;

internal class InitCommandOptionsHandler : ICommandOptionsHandler<InitCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IGitHubReleaseManager _gitHubReleaseManager;
    private readonly ILocation _location;
    private readonly IArchiveExtractor _archiveExtractor;

    public InitCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IGitHubReleaseManager gitHubReleaseManager, ILocation location, 
        IArchiveExtractor archiveExtractor)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _gitHubReleaseManager = gitHubReleaseManager ?? throw new ArgumentNullException(nameof(gitHubReleaseManager));
        _location = location ?? throw new ArgumentNullException(nameof(location));
        _archiveExtractor = archiveExtractor ?? throw new ArgumentNullException(nameof(archiveExtractor));
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
            _flowCtlLogger.Write(Resources.Commands_Init_BeginningInitialize);

            var flowSynxPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "engine");
            var flowSynxBinaryFile = _location.LookupFlowSynxBinaryFilePath(flowSynxPath);

            if (File.Exists(flowSynxBinaryFile))
            {
                _flowCtlLogger.Write(Resources.Commands_Init_FlowSynxSystemIsAlreadyInitialized);
                return;
            }

            Directory.CreateDirectory(flowSynxPath);

            var initFlowSynx = await InitFlowSynx(options.FlowSynxVersion, cancellationToken);
            if (!initFlowSynx)
                return;

            _flowCtlLogger.Write(Resources.Commands_Init_StartChangeExecutionMode);
            MakeExecutable(flowSynxBinaryFile);

            // --- Console initialization ---
            var consolePath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "console");
            var consoleBinaryFile = _location.LookupConsoleBinaryFilePath(consolePath);
            if (File.Exists(consoleBinaryFile))
            {
                _flowCtlLogger.Write(Resources.Commands_Init_FlowSynxSystemIsAlreadyInitialized);
                return;
            }
            var initConsole = await InitConsole(options.ConsoleVersion, cancellationToken);
            if (!initConsole)
                return;

            _flowCtlLogger.Write(Resources.Commands_Init_StartChangeExecutionMode);
            MakeExecutable(consoleBinaryFile);

            _flowCtlLogger.Write(string.Format(Resources.Commands_Init_FlowSynxSystemDownloadedAndInstalledSuccessfully, 
                _location.DefaultFlowSynxBinaryDirectoryName));
        }
        catch (Exception e)
        {
            _flowCtlLogger.WriteError(e.Message);
        }
    }

    private async Task<bool> InitFlowSynx(string? version, CancellationToken cancellationToken)
    {
        var flowSynxVersion = await _gitHubReleaseManager.GetLatestVersion(GitHubSettings.Organization, GitHubSettings.FlowSynxRepository);
        if (!string.IsNullOrEmpty(version))
            flowSynxVersion = version;

        flowSynxVersion = NormalizeVersion(flowSynxVersion);

        _flowCtlLogger.Write(Resources.Commands_Init_StartDownloadFlowSynxSystemBinary);
        string tempPath = Path.Combine(Path.GetTempPath(), GitHubSettings.FlowSynxArchiveTemporaryFileName);
        var flowSynxDownloadPath = await _gitHubReleaseManager.DownloadAsset(
            GitHubSettings.Organization, 
            GitHubSettings.FlowSynxRepository, 
            GitHubSettings.FlowSynxArchiveFileName, 
            tempPath, 
            flowSynxVersion);

        string tempHashPath = Path.Combine(Path.GetTempPath(), GitHubSettings.FlowSynxArchiveTemporaryHashFileName);
        var flowSynxHashDownloadPath = await _gitHubReleaseManager.DownloadAsset(
            GitHubSettings.Organization, 
            GitHubSettings.FlowSynxRepository, 
            GitHubSettings.FlowSynxArchiveHashFileName, 
            tempHashPath, 
            flowSynxVersion);

        _flowCtlLogger.Write(Resources.Commands_Init_StartValidatingFlowSynxSystemBinary);
        var isFlowSynxValid = _gitHubReleaseManager.ValidateDownloadedAsset(flowSynxDownloadPath, flowSynxHashDownloadPath);

        if (!isFlowSynxValid)
        {
            _flowCtlLogger.Write(Resources.Commands_Init_ValidatingFlowSynxSystemFailed);
            _flowCtlLogger.Write(Resources.Commands_Init_DownloadedDataCorrupted);
            return false;
        }

        _flowCtlLogger.Write(Resources.Commands_Init_StartExtractingFlowSynxSystemBinary);
        ExtractAsset(flowSynxDownloadPath, "engine", cancellationToken);
        return true;
    }

    private async Task<bool> InitConsole(string? version, CancellationToken cancellationToken)
    {
        var consoleVersion = await _gitHubReleaseManager.GetLatestVersion(GitHubSettings.Organization, GitHubSettings.ConsoleRepository);
        if (!string.IsNullOrEmpty(version))
            consoleVersion = version;

        consoleVersion = NormalizeVersion(consoleVersion);

        _flowCtlLogger.Write(Resources.Commands_Init_StartDownloadConsoleBinary);
        string tempPath = Path.Combine(Path.GetTempPath(), GitHubSettings.ConsoleArchiveTemporaryFileName);
        var consoleDownloadPath = await _gitHubReleaseManager.DownloadAsset(
            GitHubSettings.Organization,
            GitHubSettings.ConsoleRepository,
            GitHubSettings.ConsoleArchiveFileName,
            tempPath,
            consoleVersion);

        string tempHashPath = Path.Combine(Path.GetTempPath(), GitHubSettings.ConsoleArchiveTemporaryHashFileName);
        var consoleHashDownloadPath = await _gitHubReleaseManager.DownloadAsset(
            GitHubSettings.Organization,
            GitHubSettings.ConsoleRepository,
            GitHubSettings.ConsoleArchiveHashFileName,
            tempHashPath,
            consoleVersion);

        _flowCtlLogger.Write(Resources.Commands_Init_StartValidatingConsoleBinary);
        var isConsoleValid = _gitHubReleaseManager.ValidateDownloadedAsset(consoleDownloadPath, consoleHashDownloadPath);

        if (!isConsoleValid)
        {
            _flowCtlLogger.Write(Resources.Commands_Init_ValidatingConsoleFailed);
            return false;
        }

        _flowCtlLogger.Write(Resources.Commands_Init_StartExtractingConsoleBinary);
        ExtractAsset(consoleDownloadPath, "console", cancellationToken);
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

    private void MakeExecutable(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        const UnixFileMode ownershipPermissions = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                                                  UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute |
                                                  UnixFileMode.OtherRead | UnixFileMode.OtherWrite | UnixFileMode.OtherExecute;

        File.SetUnixFileMode(path, ownershipPermissions);
    }
}