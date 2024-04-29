using System.Diagnostics;
using System.Runtime.InteropServices;
using EnsureThat;
using FlowSynx.Cli.Common;
using FlowSynx.Cli.Services;

namespace FlowSynx.Cli.Commands.Update;

internal class UpdateCommandOptionsHandler : ICommandOptionsHandler<UpdateCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IVersionHandler _versionHandler;
    private readonly ILocation _location;
    private readonly IGitHub _gitHub;
    private readonly IExtractor _extractor;

    public UpdateCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IVersionHandler versionHandler, ILocation location, IGitHub gitHub,
        IExtractor extractor)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(versionHandler, nameof(versionHandler));
        EnsureArg.IsNotNull(location, nameof(location));
        EnsureArg.IsNotNull(gitHub, nameof(gitHub));
        EnsureArg.IsNotNull(extractor, nameof(extractor));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _versionHandler = versionHandler;
        _location = location;
        _gitHub = gitHub;
        _extractor = extractor;
    }

    public async Task<int> HandleAsync(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _outputFormatter.Write("Checking for FlowSynx updates...");
            await UpdateFlowSynx(options, cancellationToken);

            _outputFormatter.Write("Checking for Dashboard updates...");
            await UpdateDashboard(options, cancellationToken);

            _outputFormatter.Write("Checking for CLI updates...");
            await UpdateCli(options, cancellationToken);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }

    private async Task UpdateFlowSynx(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        var latestVersion = await _gitHub.GetLatestVersion(_gitHub.FlowSynxRepository, cancellationToken);
        var binaryFile = PathHelper.LookupFlowSynxBinaryFilePath(Path.Combine(PathHelper.DefaultFlowSynxBinaryDirectoryName, "engine"));
        var checkVersionResult = CheckVersion(binaryFile, options.FlowSynxVersion, latestVersion);
        if (checkVersionResult.IsUpdateAvailable)
        {
            var isProcessStopped = ProcessHelper.IsProcessStopped("flowsynx", ".", options.Force);
            if (!isProcessStopped)
            {
                _outputFormatter.Write($"The FlowSynx system is running. Please stop it before doing uninstall again.");
                return;
            }

            var downloadAndValidate = await DownloadAndValidateAndExtractFlowSynx(checkVersionResult.Version, cancellationToken);
            if (!downloadAndValidate)
                return;
        }
        else
        {
            _outputFormatter.Write("The current FlowSynx's version is up to dated");
        }
    }

    private async Task UpdateDashboard(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        var latestVersion = await _gitHub.GetLatestVersion(_gitHub.DashboardRepository, cancellationToken);
        var binaryFile = PathHelper.LookupDashboardBinaryFilePath(Path.Combine(PathHelper.DefaultFlowSynxBinaryDirectoryName, "dashboard"));
        var checkVersionResult = CheckVersion(binaryFile, options.DashboardVersion, latestVersion);
        if (checkVersionResult.IsUpdateAvailable)
        {
            var isProcessStopped = ProcessHelper.IsProcessStopped("dashboard", ".", options.Force);
            if (!isProcessStopped)
            {
                _outputFormatter.Write($"The Dashboard is running. Please stop it before doing uninstall again.");
                return;
            }

            var downloadAndValidate = await DownloadAndValidateAndExtractDashboard(checkVersionResult.Version, cancellationToken);
            if (!downloadAndValidate)
                return;
        }
        else
        {
            _outputFormatter.Write("The current Dashboard's version is up to dated");
        }
    }

    private async Task UpdateCli(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        var cliLatestVersion = await _gitHub.GetLatestVersion(_gitHub.CliRepository, cancellationToken);
        cliLatestVersion = _versionHandler.Normalize(cliLatestVersion);

        var cliCurrentVersion = _versionHandler.Version;

        if (_versionHandler.CheckVersions(cliLatestVersion, cliCurrentVersion))
        {
            await DownloadAndValidateAndExtractCli(cliLatestVersion, cancellationToken);
        }
        else
        {
            _outputFormatter.Write("The current CLI version is up to dated");
        }
    }

    private CheckVersionResult CheckVersion(string binaryPath, string? version, string latestVersion)
    {
        var currentVersion = _versionHandler.GetApplicationVersion(binaryPath);

        latestVersion = _versionHandler.Normalize(latestVersion);
        if (!string.IsNullOrEmpty(version))
            latestVersion = _versionHandler.Normalize(version);

        return new CheckVersionResult
        {
            IsUpdateAvailable = _versionHandler.CheckVersions(latestVersion, currentVersion),
            Version = latestVersion
        };
    }

    private async Task<bool> DownloadAndValidateAndExtractFlowSynx(string version, CancellationToken cancellationToken)
    {
        _outputFormatter.Write("Start download FlowSynx binary");
        var flowSynxDownloadPath = await _gitHub.DownloadAsset(_gitHub.FlowSynxRepository, version, _gitHub.FlowSynxArchiveFileName, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write("Start validating FlowSynx binary");
        var isFlowSynxValid = await _gitHub.ValidateDownloadedAsset(flowSynxDownloadPath, _gitHub.FlowSynxRepository, version, _gitHub.FlowSynxArchiveHashFileName, cancellationToken);

        if (isFlowSynxValid)
        {
            _outputFormatter.Write("Start extracting FlowSynx binary");
            ExtractAsset(flowSynxDownloadPath, "engine", cancellationToken);
            return true;
        }

        _outputFormatter.Write("Validating download - Fail!");
        _outputFormatter.Write("The downloaded data may has been corrupted!");
        return false;
    }

    private async Task<bool> DownloadAndValidateAndExtractDashboard(string version, CancellationToken cancellationToken)
    {
        _outputFormatter.Write("Start download Dashboard binary");
        var dashboardDownloadPath = await _gitHub.DownloadAsset(_gitHub.DashboardRepository, version, _gitHub.DashboardArchiveFileName, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write("Start validating Dashboard binary");
        var isDashboardValid = await _gitHub.ValidateDownloadedAsset(dashboardDownloadPath, _gitHub.DashboardRepository, version, _gitHub.DashboardArchiveHashFileName, cancellationToken);

        if (isDashboardValid)
        {
            _outputFormatter.Write("Start extracting Dashboard binary");
            ExtractAsset(dashboardDownloadPath, "dashboard", cancellationToken);
            return true;
        }

        _outputFormatter.Write("Validating download - Fail!");
        _outputFormatter.Write("The downloaded data may has been corrupted!");
        return false;
    }

    private async Task DownloadAndValidateAndExtractCli(string version, CancellationToken cancellationToken)
    {
        _outputFormatter.Write("Start download Cli binary");
        var cliDownloadPath = await _gitHub.DownloadAsset(_gitHub.CliRepository, version, _gitHub.FlowSynxCliArchiveFileName, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write("Start validating Cli binary");
        var isCliValid = await _gitHub.ValidateDownloadedAsset(cliDownloadPath, _gitHub.CliRepository, version, _gitHub.FlowSynxCliArchiveHashFileName, cancellationToken);

        if (isCliValid)
            ExtractFlowSynxCli(cliDownloadPath, cancellationToken);

        _outputFormatter.Write("Validating download - Fail!");
        _outputFormatter.Write("The downloaded data may has been corrupted!");
    }

    private void ExtractAsset(string sourcePath, string destinationPathName, CancellationToken cancellationToken)
    {
        var extractTarget = Path.Combine(PathHelper.DefaultFlowSynxBinaryDirectoryName, destinationPathName, "downloadedFiles");
        var destinationPath = Path.Combine(PathHelper.DefaultFlowSynxBinaryDirectoryName, destinationPathName);

        Directory.CreateDirectory(extractTarget);
        _extractor.ExtractFile(sourcePath, extractTarget);
        Directory.CreateDirectory(destinationPath);
        PathHelper.CopyFilesRecursively(extractTarget, destinationPath, cancellationToken);
        Directory.Delete(extractTarget, true);
    }

    private async void ExtractFlowSynxCli(string sourcePath, CancellationToken cancellationToken)
    {
        const string extractTarget = "./downloadedFiles";
        Directory.CreateDirectory(extractTarget);

        _extractor.ExtractFile(sourcePath, extractTarget);
        File.Delete(sourcePath);

        var synxUpdateExeFile = Path.GetFullPath(PathHelper.LookupSynxBinaryFilePath(extractTarget));
        var files = Directory
            .GetFiles(extractTarget, "*.*", SearchOption.AllDirectories)
            .Where(name => !string.Equals(Path.GetFullPath(name), synxUpdateExeFile, StringComparison.InvariantCultureIgnoreCase));

        foreach (var newPath in files)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            File.Copy(newPath, newPath.Replace(extractTarget, "."), true);
        }

        var synxExeFile = Path.GetFullPath(PathHelper.LookupSynxBinaryFilePath(_location.RootLocation));
        await SelfUpdate(synxUpdateExeFile, synxExeFile, cancellationToken);
    }
    
    private async Task SelfUpdate(string updateFile, string selfFile, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(updateFile);

        var selfFileName = Path.GetFileName(selfFile);
        var directoryName = Path.GetDirectoryName(selfFile);
        var downloadedFilesPath = Path.GetDirectoryName(updateFile);
        if (string.IsNullOrEmpty(directoryName))
            return;

        string selfWithoutExt = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(selfFile));
        StreamHelper.SaveStreamToFile(stream, selfWithoutExt + PathHelper.GetUpdateFilePath());

        string updateExeFile = selfWithoutExt + PathHelper.GetUpdateFilePath();
        string scriptFile = selfWithoutExt + PathHelper.GetScriptFilePath();

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
            _outputFormatter.Write("The FlowSynx system updated successfully.");
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
            System.Environment.Exit(0);
        }
    }
}