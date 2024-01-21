using System.IO.Compression;
using System.Net;
using System.Text;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;

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

    public InitCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner, 
        IVersion version, ILocation location, IOperatingSystemInfo operatingSystemInfo)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(version, nameof(version));
        EnsureArg.IsNotNull(location, nameof(location));
        EnsureArg.IsNotNull(operatingSystemInfo, nameof(operatingSystemInfo));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _version = version;
        _location = location;
        _operatingSystemInfo = operatingSystemInfo;
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
        {
            ExtractTarGz(sourcePath, destinationPath);
        }
        else
        {
            ZipFile.ExtractToDirectory(sourcePath, destinationPath, true);
        }
    }

    public static void ExtractTarGz(string filename, string outputDir)
    {
        void ReadExactly(Stream stream, byte[] buffer, int count)
        {
            var total = 0;
            while (true)
            {
                int n = stream.Read(buffer, total, count - total);
                total += n;
                if (total == count)
                    return;
            }
        }

        void SeekExactly(Stream stream, byte[] buffer, int count)
        {
            ReadExactly(stream, buffer, count);
        }

        using var fs = File.OpenRead(filename);
        using (var stream = new GZipStream(fs, CompressionMode.Decompress))
        {
            var buffer = new byte[1024];
            while (true)
            {
                ReadExactly(stream, buffer, 100);
                var name = Encoding.ASCII.GetString(buffer, 0, 100).Split('\0')[0];
                if (string.IsNullOrWhiteSpace(name))
                    break;

                SeekExactly(stream, buffer, 24);

                ReadExactly(stream, buffer, 12);
                var sizeString = Encoding.ASCII.GetString(buffer, 0, 12).Split('\0')[0];
                var size = Convert.ToInt64(sizeString, 8);

                SeekExactly(stream, buffer, 209);

                ReadExactly(stream, buffer, 155);
                var prefix = Encoding.ASCII.GetString(buffer, 0, 155).Split('\0')[0];
                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    name = prefix + name;
                }

                SeekExactly(stream, buffer, 12);

                var output = Path.GetFullPath(Path.Combine(outputDir, name));
                if (!Directory.Exists(Path.GetDirectoryName(output)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(output));
                }
                using (var outfs = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    var total = 0;
                    while (true)
                    {
                        var next = Math.Min(buffer.Length, (int)size - total);
                        ReadExactly(stream, buffer, next);
                        outfs.Write(buffer, 0, next);
                        total += next;
                        if (total == size)
                            break;
                    }
                }

                var offset = 512 - ((int)size % 512);
                if (offset == 512)
                    offset = 0;

                SeekExactly(stream, buffer, offset);
            }
        }
    }
}