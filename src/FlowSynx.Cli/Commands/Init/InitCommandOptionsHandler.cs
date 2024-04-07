using EnsureThat;
using FlowSynx.Cli.Common;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.IO.Compression;

namespace FlowSynx.Cli.Commands.Init;

internal class InitCommandOptionsHandler : ICommandOptionsHandler<InitCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IVersion _version;
    private readonly IOperatingSystemInfo _operatingSystemInfo;
    private readonly Func<CompressType, ICompression> _compressionFactory;

    public InitCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IVersion version, IOperatingSystemInfo operatingSystemInfo,
        Func<CompressType, ICompression> compressionFactory)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(version, nameof(version));
        EnsureArg.IsNotNull(operatingSystemInfo, nameof(operatingSystemInfo));
        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _version = version;
        _operatingSystemInfo = operatingSystemInfo;
        _compressionFactory = compressionFactory;
    }

    private string FlowSynxArchiveFileName => $"flowsynx-{ArchiveName.ToLower()}";
    private string FlowSynxArchiveHashFileName => $"flowsynx-{ArchiveName.ToLower()}.sha256";
    private string ArchiveName => $"{_operatingSystemInfo.Type}-{_operatingSystemInfo.Architecture}.{Extension}";
    private string Extension => string.Equals(_operatingSystemInfo.Type, "windows", StringComparison.OrdinalIgnoreCase) ? "zip" : "tar.gz";

    public async Task<int> HandleAsync(InitCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(cancellationToken));
        return 0;
    }

    private async Task Execute(CancellationToken cancellationToken)
    {
        try
        {
            _outputFormatter.Write("Beginning Initialize...");

            var flowSynxPath = Path.Combine(PathHelper.DefaultFlowSynxDirectoryName, "engine");
            var flowSynxBinaryFile = PathHelper.LookupFlowSynxBinaryFilePath(flowSynxPath);
            if (File.Exists(flowSynxBinaryFile))
            {
                _outputFormatter.Write("The FlowSynx engine is already initialized.");
                _outputFormatter.Write("You can use command 'Synx update' to check and update the FlowSynx.");
                return;
            }
            Directory.CreateDirectory(flowSynxPath);
            var initialized = await Init(cancellationToken);

            if (!initialized)
                return;

            _outputFormatter.Write("Starting to change the execution mode of FlowSynx.");
            PathHelper.MakeExecutable(flowSynxBinaryFile);

            _outputFormatter.Write("FlowSynx engine is downloaded and installed successfully.");
        }
        catch (Exception e)
        {
            _outputFormatter.WriteError(e.Message);
        }
    }

    private async Task<bool> Init(CancellationToken cancellationToken)
    {
        var currentVersion = _version.Version;

        _outputFormatter.Write("Starting download FlowSynx binary");
        var flowSynxDownloadPath = await DownloadFlowSynxAsset(currentVersion, Path.GetTempPath(), cancellationToken);

        _outputFormatter.Write("Starting validating FlowSynx binary");
        var isFlowSynxValid = await ValidateFlowSynxDownloadedAsset(flowSynxDownloadPath, currentVersion, cancellationToken);

        if (isFlowSynxValid)
        {
            _outputFormatter.Write("Starting extract FlowSynx binary");
            await Task.Run(() => ExtractFlowSynx(flowSynxDownloadPath, cancellationToken), cancellationToken);
            return true;
        }

        _outputFormatter.Write("Validating download - Fail!");
        _outputFormatter.Write("The downloaded data may has been corrupted!");
        return false;
    }

    private async Task<bool> ValidateFlowSynxDownloadedAsset(string path, string latestVersion, CancellationToken cancellationToken)
    {
        var expectedHash = await DownloadFlowSynxHashAsset(latestVersion, cancellationToken);
        var downloadHash = HashHelper.ComputeSha256Hash(path);

        if (string.Equals(downloadHash.Trim(), expectedHash.Trim(), StringComparison.CurrentCultureIgnoreCase))
            return true;

        File.Delete(path);
        return false;
    }

    private Task ExtractFlowSynx(string sourcePath, CancellationToken cancellationToken)
    {
        var extractTarget = Path.Combine(PathHelper.DefaultFlowSynxDirectoryName, "engine", "downloadedFiles");
        var enginePath = Path.Combine(PathHelper.DefaultFlowSynxDirectoryName, "engine");

        ExtractFile(sourcePath, extractTarget);
        Directory.CreateDirectory(enginePath);

        foreach (var newPath in Directory.GetFiles(extractTarget, "*.*", SearchOption.AllDirectories))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            File.Copy(newPath, newPath.Replace(extractTarget, enginePath), true);
        }

        Directory.Delete(extractTarget, true);
        return Task.CompletedTask;
    }

    private async Task<string> DownloadFlowSynxAsset(string version, string destinationPath, CancellationToken cancellationToken)
    {
        var uri = $"https://github.com/{GitHubHelper.Organization}/{GitHubHelper.FlowSynxRepository}/releases/download/v{version}/{FlowSynxArchiveFileName}";
        var stream = await NetHelper.DownloadFile(uri, cancellationToken);
        var path = Path.Combine(destinationPath, FlowSynxArchiveFileName);
        StreamHelper.SaveStreamToFile(stream, path);
        return path;
    }

    private async Task<string> DownloadFlowSynxHashAsset(string version, CancellationToken cancellationToken)
    {
        var uri = $"https://github.com/{GitHubHelper.Organization}/{GitHubHelper.FlowSynxRepository}/releases/download/v{version}/{FlowSynxArchiveHashFileName}";
        var stream = await NetHelper.DownloadFile(uri, cancellationToken);
        return await HashHelper.GetAssetHashCode(stream, cancellationToken);
    }
    
    private void ExtractFile(string sourcePath, string destinationPath)
    {
        var compressEntry = new CompressEntry
        {
            Stream = File.OpenWrite(sourcePath),
            Name = Path.GetFileName(sourcePath),
            ContentType = ""
        };

        if (Extension == "tar.gz")
            _compressionFactory(CompressType.GZip).Decompress(compressEntry, destinationPath);
        else
            _compressionFactory(CompressType.Zip).Decompress(compressEntry, destinationPath);
    }
}