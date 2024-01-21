using System.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.IO.Compression;

namespace FlowSynx.Cli.Commands.Execute;

internal class InitCommand : BaseCommand<InitCommandOptions, InitCommandOptionsHandler>
{
    public InitCommand() : base("init", "Initialize FlowSynx engine")
    {
    }
}

internal class InitCommandOptions : ICommandOptions
{

}

internal class InitCommandOptionsHandler : ICommandOptionsHandler<InitCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IVersion _version;
    private readonly ILocation _location;
    private readonly IOperatingSystemInfo _operatingSystemInfo;
    private readonly IZipFile _zipFile;
    private readonly IGZipFile _gZipFile;

    public InitCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner, 
        IVersion version, ILocation location, IOperatingSystemInfo operatingSystemInfo,
        IZipFile zipFile, IGZipFile gZipFile)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(version, nameof(version));
        EnsureArg.IsNotNull(location, nameof(location));
        EnsureArg.IsNotNull(operatingSystemInfo, nameof(operatingSystemInfo));
        EnsureArg.IsNotNull(zipFile, nameof(zipFile));
        EnsureArg.IsNotNull(gZipFile, nameof(gZipFile));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _version = version;
        _location = location;
        _operatingSystemInfo = operatingSystemInfo;
        _zipFile = zipFile;
        _gZipFile = gZipFile;
    }

    public async Task<int> HandleAsync(InitCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await InitializeFlowSynx(options, cancellationToken));
        return 0;
    }

    private async Task InitializeFlowSynx(InitCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var flowSynxPath = Path.Combine(_location.RootLocation, "bin");
            Directory.CreateDirectory(flowSynxPath);
            await DownloadBinary("0.2.0", flowSynxPath, cancellationToken);
        }
        catch (Exception e)
        {
            _outputFormatter.WriteError(e.Message);
        }
    }

    private async Task DownloadBinary(string version, string installPath, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        var archiveName = ArchiveName.ToLower();
        var uri = $"https://github.com/{GitHubOrganization}/{GitHubRepository}/releases/download/{version}/{archiveName}";
        var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
        using var response = await client.SendAsync(message, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new Exception($"Version not found from url: {uri}");

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Download failed with {response.StatusCode.ToString()}");

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var archivePath = Path.Combine(installPath, archiveName);
        await using (var fs = new FileStream(archivePath, FileMode.OpenOrCreate))
        {
            await stream.CopyToAsync(fs, cancellationToken);
        }

        ExtractFile(archivePath, installPath);
        File.Delete(archivePath);
        _outputFormatter.Write(@"FlowSynx engine is downloaded and installed successfully.");
    }

    private string ArchiveName => $"FlowSynx-{_operatingSystemInfo.Type}-{_operatingSystemInfo.Architecture}.{Extension}";
    private string GitHubOrganization => "FlowSynx";
    private string GitHubRepository => "TestWorkflow";
    private string Extension => string.Equals(_operatingSystemInfo.Type, "windows", StringComparison.OrdinalIgnoreCase) ? "zip" : "tar.gz";

    private void ExtractFile(string sourcePath, string destinationPath)
    {
        if (Extension == "tar.gz")
            _gZipFile.Decompression(sourcePath, destinationPath, true);
        else
            _zipFile.Decompression(sourcePath, destinationPath, true);
    }
}