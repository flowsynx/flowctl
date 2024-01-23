using System.Net;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using System.Security.Cryptography;
using System.Text;
using FlowSynx.IO.Compression;
using FlowSynx.IO.Serialization;
using System.Threading;

namespace FlowSynx.Cli.Commands.Version;

internal class UpdateCommand : BaseCommand<UpdateCommandOptions, UpdateCommandOptionsHandler>
{
    public UpdateCommand() : base("update", "Configuration management")
    {

    }
}

internal class UpdateCommandOptions : ICommandOptions
{

}

internal class UpdateCommandOptionsHandler : ICommandOptionsHandler<UpdateCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IHttpRequestService _httpRequestService;
    private readonly IVersion _version;
    private readonly IOperatingSystemInfo _operatingSystemInfo;
    private readonly IZipFile _zipFile;
    private readonly IGZipFile _gZipFile;
    private readonly IDeserializer _deserializer;

    public UpdateCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IHttpRequestService httpRequestService, IVersion version, IOperatingSystemInfo operatingSystemInfo,
        IZipFile zipFile, IGZipFile gZipFile, IDeserializer deserializer)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));
        EnsureArg.IsNotNull(version, nameof(version));
        EnsureArg.IsNotNull(operatingSystemInfo, nameof(operatingSystemInfo));
        EnsureArg.IsNotNull(zipFile, nameof(zipFile));
        EnsureArg.IsNotNull(gZipFile, nameof(gZipFile));
        EnsureArg.IsNotNull(deserializer, nameof(deserializer));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _httpRequestService = httpRequestService;
        _version = version;
        _operatingSystemInfo = operatingSystemInfo;
        _zipFile = zipFile;
        _gZipFile = gZipFile;
        _deserializer = deserializer;
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
            _outputFormatter.Write("Checking for updates...");

            var latestVersion = await GetLatestVersion(FlowSynxCliGitHubRepository);
            var currentVersion = "0.2.0";

            if (IsUpdateAvailable(latestVersion, currentVersion))
            {
                _outputFormatter.Write($"An update is available! Version {latestVersion}");
                _outputFormatter.Write("Beginning update...");

                var flowSynxDownloadPath = await DownloadFlowSynxAsset(latestVersion, Path.GetTempPath(), cancellationToken);
                var isFlowSynxValid = await ValidateFlowSynxDownloadedAsset(flowSynxDownloadPath, latestVersion, cancellationToken);

                var flowSynxCliDownloadPath = await DownloadFlowSynxCliAsset(latestVersion, Path.GetTempPath(), cancellationToken);
                var isFlowSynxCliValid = await ValidateFlowSynxCliDownloadedAsset(flowSynxCliDownloadPath, latestVersion, cancellationToken);

                if (isFlowSynxValid && isFlowSynxCliValid)
                {
                    await Task
                        .Run(() => ExtractFlowSynx(flowSynxDownloadPath, cancellationToken), cancellationToken)
                        .ContinueWith(t =>
                        {
                            if (t.IsCanceled || t.IsFaulted)
                            {
                                var exception = t.Exception;
                                throw new Exception(exception?.Message);
                            }
                            ExtractFlowSynxCli(flowSynxCliDownloadPath, cancellationToken);
                        }, cancellationToken);
                }
                else
                {
                    _outputFormatter.Write("Validating download - Fail!");
                    _outputFormatter.Write("The downloaded data may has been corrupted!");
                }
            }
            else
            {
                _outputFormatter.Write("The current version is up to dated");
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
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

    private async Task<bool> ValidateFlowSynxCliDownloadedAsset(string path, string latestVersion, CancellationToken cancellationToken)
    {
        var expectedHash = await DownloadFlowSynxCliHashAsset(latestVersion, cancellationToken);
        var downloadHash = ComputeSha256Hash(path);

        if (string.Equals(downloadHash.Trim(), expectedHash.Trim(), StringComparison.CurrentCultureIgnoreCase)) 
            return true;

        File.Delete(path);
        return false;
    }

    private Task ExtractFlowSynxCli(string path, CancellationToken cancellationToken)
    {
        var extractTarget = @"./downloadedFiles";
        ExtractFile(path, extractTarget);

        foreach (var newPath in Directory.GetFiles(extractTarget, "*.*", SearchOption.AllDirectories))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            File.Copy(newPath, newPath.Replace(extractTarget, "."), true);
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

    private async Task<string> DownloadFlowSynxCliAsset(string version, string destinationPath, CancellationToken cancellationToken)
    {
        var uri = $"https://github.com/{FlowSynxGitHubOrganization}/{FlowSynxCliGitHubRepository}/releases/download/{version}/{FlowSynxCliArchiveFileName}";
        var stream = await DownloadFile(uri, cancellationToken);
        var path = Path.Combine(destinationPath, FlowSynxCliArchiveFileName);
        SaveStreamToFile(stream, path);
        return path;
    }

    private async Task<string> DownloadFlowSynxHashAsset(string version, CancellationToken cancellationToken)
    {
        var uri = $"https://github.com/{FlowSynxGitHubOrganization}/{FlowSynxGitHubRepository}/releases/download/{version}/{FlowSynxArchiveHashFileName}";
        var stream = await DownloadFile(uri, cancellationToken);
        return await GetAssetHashCode(stream, cancellationToken);
    }

    private async Task<string> DownloadFlowSynxCliHashAsset(string version, CancellationToken cancellationToken)
    {
        var uri = $"https://github.com/{FlowSynxGitHubOrganization}/{FlowSynxCliGitHubRepository}/releases/download/{version}/{FlowSynxCliArchiveHashFileName}";
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

    private string FlowSynxArchiveFileName => $"flowSynx-{ArchiveName.ToLower()}";
    private string FlowSynxArchiveHashFileName => $"flowSynx-{ArchiveName.ToLower()}.sha256";
    private string FlowSynxCliArchiveFileName => $"flowSynx-{ArchiveName.ToLower()}";
    private string FlowSynxCliArchiveHashFileName => $"flowSynx-{ArchiveName.ToLower()}.sha256";
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
    
    private bool IsUpdateAvailable(string latestVersion, string currentVersion)
    {
        if (string.IsNullOrEmpty(latestVersion)) return false;

        var current = new System.Version(currentVersion);
        var latest = new System.Version(latestVersion);
        return latest > current;
    }

    private void ExtractFile(string sourcePath, string destinationPath)
    {
        if (Extension == "tar.gz")
            _gZipFile.Decompression(sourcePath, destinationPath, true);
        else
            _zipFile.Decompression(sourcePath, destinationPath, true);
    }
}

internal class GitHubTag
{
    public required string Name { get; set; }
}