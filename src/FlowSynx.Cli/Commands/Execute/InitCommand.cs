using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.IO.Compression;
using FlowSynx.IO.Serialization;

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
    private readonly IDeserializer _deserializer;

    public InitCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IVersion version, ILocation location, IOperatingSystemInfo operatingSystemInfo,
        IZipFile zipFile, IGZipFile gZipFile, IDeserializer deserializer)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(version, nameof(version));
        EnsureArg.IsNotNull(location, nameof(location));
        EnsureArg.IsNotNull(operatingSystemInfo, nameof(operatingSystemInfo));
        EnsureArg.IsNotNull(zipFile, nameof(zipFile));
        EnsureArg.IsNotNull(gZipFile, nameof(gZipFile));
        EnsureArg.IsNotNull(deserializer, nameof(deserializer));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _version = version;
        _location = location;
        _operatingSystemInfo = operatingSystemInfo;
        _zipFile = zipFile;
        _gZipFile = gZipFile;
        _deserializer = deserializer;
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

            var flowSynxPath = Path.Combine(_location.RootLocation, "bin");
            var look = LookupBinaryFilePath(flowSynxPath);
            if (File.Exists(look))
            {
                _outputFormatter.Write("The FlowSynx engine is already Initialized.");
                _outputFormatter.Write("You can use command 'Synx update' to check and update the FlowSynx.");
                return;
            }
            Directory.CreateDirectory(flowSynxPath);
            await Init(cancellationToken);
            _outputFormatter.Write(@"FlowSynx engine is downloaded and installed successfully.");
        }
        catch (Exception e)
        {
            _outputFormatter.WriteError(e.Message);
        }
    }

    private async Task Init(CancellationToken cancellationToken)
    {
        var latestVersion = await GetLatestVersion(FlowSynxCliGitHubRepository);
        var flowSynxDownloadPath = await DownloadFlowSynxAsset(latestVersion, Path.GetTempPath(), cancellationToken);
        var isFlowSynxValid = await ValidateFlowSynxDownloadedAsset(flowSynxDownloadPath, latestVersion, cancellationToken);

        if (isFlowSynxValid)
        {
            await Task.Run(() => ExtractFlowSynx(flowSynxDownloadPath, cancellationToken), cancellationToken);
        }
        else
        {
            _outputFormatter.Write("Validating download - Fail!");
            _outputFormatter.Write("The downloaded data may has been corrupted!");
        }
    }

    private async Task<bool> ValidateFlowSynxDownloadedAsset(string path, string latestVersion, CancellationToken cancellationToken)
    {
        var expectedHash = await DownloadFlowSynxHashAsset(latestVersion, cancellationToken);
        var downloadHash = ComputeSha256Hash(path);

        if (string.Equals(downloadHash.Trim(), expectedHash.Trim(), StringComparison.CurrentCultureIgnoreCase))
            return true;

        File.Delete(path);
        return false;
    }

    private Task ExtractFlowSynx(string sourcePath, CancellationToken cancellationToken)
    {
        var extractTarget = @"./downloadedFiles";
        ExtractFile(sourcePath, extractTarget);

        Directory.CreateDirectory("./bin");

        foreach (var newPath in Directory.GetFiles(extractTarget, "*.*", SearchOption.AllDirectories))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            File.Copy(newPath, newPath.Replace(extractTarget, "./bin"), true);
        }

        Directory.Delete(extractTarget, true);
        return Task.CompletedTask;
    }

    private async Task<string> GetLatestVersion(string repositoryName)
    {
        var httpClient = new HttpClient();
        var uri = $"https://api.github.com/repos/{FlowSynxGitHubOrganization}/{repositoryName}/tags";
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(uri)
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var tags = _deserializer.Deserialize<List<GitHubTag>>(responseBody);
        return tags.Count <= 0 ? string.Empty : tags.First().Name;
    }

    private async Task<string> DownloadFlowSynxAsset(string version, string destinationPath, CancellationToken cancellationToken)
    {
        var uri = $"https://github.com/{FlowSynxGitHubOrganization}/{FlowSynxGitHubRepository}/releases/download/{version}/{FlowSynxArchiveFileName}";
        var stream = await DownloadFile(uri, cancellationToken);
        var path = Path.Combine(destinationPath, FlowSynxArchiveFileName);
        SaveStreamToFile(stream, path);
        return path;
    }

    private async Task<string> DownloadFlowSynxHashAsset(string version, CancellationToken cancellationToken)
    {
        var uri = $"https://github.com/{FlowSynxGitHubOrganization}/{FlowSynxGitHubRepository}/releases/download/{version}/{FlowSynxArchiveHashFileName}";
        var stream = await DownloadFile(uri, cancellationToken);
        return await GetAssetHashCode(stream, cancellationToken);
    }

    private async Task<Stream> DownloadFile(string uri, CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
        var response = await client.SendAsync(message, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new Exception($"Version not found from url: {uri}");

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Download failed with {response.StatusCode.ToString()}");

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    private void SaveStreamToFile(Stream stream, string path)
    {
        var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        stream.CopyTo(fileStream);
        fileStream.Dispose();
    }

    private async Task<string> GetAssetHashCode(Stream stream, CancellationToken cancellationToken)
    {
        using var sr = new StreamReader(stream);
        var content = await sr.ReadToEndAsync(cancellationToken);
        return content.Split('*')[0].Trim();
    }








    //private async Task<string> GetLatestVersion(string repositoryName)
    //{
    //    var httpClient = new HttpClient();
    //    var uri = $"https://api.github.com/repos/{FlowSynxGitHubOrganization}/{repositoryName}/tags";
    //    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    //    httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

    //    var request = new HttpRequestMessage
    //    {
    //        Method = HttpMethod.Get,
    //        RequestUri = new Uri(uri)
    //    };

    //    var response = await httpClient.SendAsync(request);
    //    response.EnsureSuccessStatusCode();

    //    var responseBody = await response.Content.ReadAsStringAsync();
    //    var tags = _deserializer.Deserialize<List<GitHubTag>>(responseBody);
    //    return tags.Count <= 0 ? string.Empty : tags.First().Name;
    //}

    //private async Task<string> DownloadBinary(string repositoryName, string version, string archiveFileName, string destinationPath, CancellationToken cancellationToken)
    //{
    //    using var client = new HttpClient();
    //    var uri = $"https://github.com/{FlowSynxGitHubOrganization}/{repositoryName}/releases/download/{version}/{archiveFileName}";
    //    var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
    //    using var response = await client.SendAsync(message, cancellationToken);

    //    if (response.StatusCode == HttpStatusCode.NotFound)
    //        throw new Exception($"Version not found from url: {uri}");

    //    if (response.StatusCode != HttpStatusCode.OK)
    //        throw new Exception($"Download failed with {response.StatusCode.ToString()}");

    //    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
    //    var archivePath = Path.Combine(destinationPath, archiveFileName);
    //    await using var fs = new FileStream(archivePath, FileMode.OpenOrCreate);
    //    await stream.CopyToAsync(fs, cancellationToken);
    //    return archivePath;
    //}

    //private async Task<string> GetLatestHashFile(string repositoryName, string version, string archiveFileName, CancellationToken cancellationToken)
    //{
    //    using var client = new HttpClient();
    //    var uri = $"https://github.com/{FlowSynxGitHubOrganization}/{repositoryName}/releases/download/{version}/{archiveFileName}";
    //    var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
    //    using var response = await client.SendAsync(message, cancellationToken);

    //    if (response.StatusCode == HttpStatusCode.NotFound)
    //        throw new Exception($"Version not found from url: {uri}");

    //    if (response.StatusCode != HttpStatusCode.OK)
    //        throw new Exception($"Download failed with {response.StatusCode.ToString()}");

    //    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
    //    using var sr = new StreamReader(stream);
    //    var content = await sr.ReadToEndAsync(cancellationToken);
    //    return content.Split('*')[0].Trim();
    //}

    private string FlowSynxArchiveFileName => $"flowSynx-{ArchiveName.ToLower()}";
    private string FlowSynxArchiveHashFileName => $"flowSynx-{ArchiveName.ToLower()}.sha256";
    private string ArchiveName => $"{_operatingSystemInfo.Type}-{_operatingSystemInfo.Architecture}.{Extension}";
    private string Extension => string.Equals(_operatingSystemInfo.Type, "windows", StringComparison.OrdinalIgnoreCase) ? "zip" : "tar.gz";
    private string FlowSynxGitHubOrganization => "FlowSynx";
    private string FlowSynxGitHubRepository => "TestWorkflow";
    private string FlowSynxCliGitHubRepository => "TestWorkflow";

    private string ComputeSha256Hash(string filePath)
    {
        var file = new FileStream(filePath, FileMode.Open);
        using SHA256 sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(file);
        file.Close();

        var builder = new StringBuilder();

        foreach (var t in bytes)
            builder.Append(t.ToString("x2"));

        return builder.ToString();
    }

    private void ExtractFile(string sourcePath, string destinationPath)
    {
        if (Extension == "tar.gz")
            _gZipFile.Decompression(sourcePath, destinationPath, true);
        else
            _zipFile.Decompression(sourcePath, destinationPath, true);
    }

    private string LookupBinaryFilePath(string path)
    {
        var binFileName = "FlowSynx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }
}

internal class GitHubTag
{
    public required string Name { get; set; }
}