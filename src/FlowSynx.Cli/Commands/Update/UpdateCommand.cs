using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using System.Security.Cryptography;
using System.Text;
using FlowSynx.Cli.Services;
using FlowSynx.IO.Compression;
using FlowSynx.IO.Serialization;
using System.CommandLine;
using System.Xml.Linq;
using System.IO;

namespace FlowSynx.Cli.Commands.Update;

internal class UpdateCommand : BaseCommand<UpdateCommandOptions, UpdateCommandOptionsHandler>
{
    public UpdateCommand() : base("update", "Update FlowSynx system and Cli")
    {
        var forceOption = new Option<bool>(new[] { "--force" }, getDefaultValue: () => false, description: "Force terminate FlowSynx system if it is running");

        AddOption(forceOption);
    }
}

internal class UpdateCommandOptions : ICommandOptions
{
    public bool Force { get; set; }
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
    private readonly ILocation _location;

    public UpdateCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IHttpRequestService httpRequestService, IVersion version, IOperatingSystemInfo operatingSystemInfo,
        IZipFile zipFile, IGZipFile gZipFile, IDeserializer deserializer, ILocation location)
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
        _location = location;
    }

    public async Task<int> HandleAsync(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        DownloadUpdateResult result = new DownloadUpdateResult();
        await _spinner.DisplayLineSpinnerAsync(async () =>
        {
            result = await DownloadAndValidate(options, cancellationToken);
        });

        if (result.Success)
        {
            try
            {
                await Task
                    .Run(() => ExtractFlowSynx(result.FlowSynxDownloadPath, cancellationToken), cancellationToken)
                    .ContinueWith(t =>
                    {
                        if (t.IsCanceled || t.IsFaulted)
                        {
                            var exception = t.Exception;
                            throw new Exception(exception?.Message);
                        }
                        ExtractFlowSynxCli(result.FlowSynxCliDownloadPath, cancellationToken);
                    }, cancellationToken);
            }
            catch (Exception ex)
            {
                _outputFormatter.WriteError(ex.Message);
            }
        }
        return 0;
    }

    private async Task<DownloadUpdateResult> DownloadAndValidate(UpdateCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _outputFormatter.Write("Checking for updates...");

            if (options.Force)
            {
                TerminateProcess("FlowSynx", ".");
            }
            else
            {
                if (IsProcessRunning("FlowSynx", "."))
                {
                    _outputFormatter.Write("The FlowSynx engine is running. Please stop it by run the command: 'Synx stop', and try update again.");
                    return new DownloadUpdateResult { Success = false };
                }
            }

            var latestVersion = await GetLatestVersion(FlowSynxCliGitHubRepository);
            var currentVersion = _version.Version;

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
                    return new DownloadUpdateResult
                    {
                        Success = true,
                        FlowSynxDownloadPath = flowSynxDownloadPath,
                        FlowSynxCliDownloadPath = flowSynxCliDownloadPath
                    };
                }
                else
                {
                    _outputFormatter.Write("Validating download - Fail!");
                    _outputFormatter.Write("The downloaded data may has been corrupted!");
                    return new DownloadUpdateResult { Success = false };
                }
            }
            else
            {
                _outputFormatter.Write("The current version is up to dated");
                return new DownloadUpdateResult { Success = false };
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
            return new DownloadUpdateResult { Success = false };
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
        var extractTarget = Path.Combine(DefaultFlowSynxDirName, "engine", "downloadedFiles");
        var enginePath = Path.Combine(DefaultFlowSynxDirName, "engine");

        ExtractFile(sourcePath, extractTarget);
        File.Delete(sourcePath);

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

    private async Task<bool> ValidateFlowSynxCliDownloadedAsset(string path, string latestVersion, CancellationToken cancellationToken)
    {
        var expectedHash = await DownloadFlowSynxCliHashAsset(latestVersion, cancellationToken);
        var downloadHash = ComputeSha256Hash(path);

        if (string.Equals(downloadHash.Trim(), expectedHash.Trim(), StringComparison.CurrentCultureIgnoreCase))
            return true;

        File.Delete(path);
        return false;
    }

    private Task ExtractFlowSynxCli(string sourcePath, CancellationToken cancellationToken)
    {
        var extractTarget = @"./downloadedFiles";
        ExtractFile(sourcePath, extractTarget);
        File.Delete(sourcePath);

        var synxUpdateExeFile = Path.GetFullPath(LookupBinaryFilePath(extractTarget));
        var files = Directory
                                    .GetFiles(extractTarget, "*.*", SearchOption.AllDirectories)
                                    .Where(name => !string.Equals(Path.GetFullPath(name), synxUpdateExeFile, StringComparison.InvariantCultureIgnoreCase));

        foreach (var newPath in files)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            File.Copy(newPath, newPath.Replace(extractTarget, "."), true);
        }

        var synxExeFile = Path.GetFullPath(LookupBinaryFilePath(_location.RootLocation));
        SelfUpdate(synxUpdateExeFile, synxExeFile);
        
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
    private string FlowSynxCliArchiveFileName => $"synx-{ArchiveName.ToLower()}";
    private string FlowSynxCliArchiveHashFileName => $"synx-{ArchiveName.ToLower()}.sha256";
    private string ArchiveName => $"{_operatingSystemInfo.Type}-{_operatingSystemInfo.Architecture}.{Extension}";
    private string Extension => string.Equals(_operatingSystemInfo.Type, "windows", StringComparison.OrdinalIgnoreCase) ? "zip" : "tar.gz";
    private string FlowSynxGitHubOrganization => "FlowSynx";
    private string FlowSynxGitHubRepository => "FlowSynx";
    private string FlowSynxCliGitHubRepository => "Cli";
    private string UserProfilePath => System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
    private string DefaultFlowSynxDirName => Path.Combine(UserProfilePath, ".flowsynx");

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

    private bool IsProcessRunning(string processName, string machineAddress)
    {
        var processes = Process.GetProcessesByName(processName, machineAddress);
        return processes.Length != 0;
    }

    private void TerminateProcess(string processName, string machineAddress)
    {
        var processes = Process.GetProcessesByName(processName, machineAddress);

        if (processes.Length == 0) return;

        try
        {
            foreach (var process in processes)
                process.Kill();

            _outputFormatter.Write("The FlowSynx system was stopped successfully.");
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }

    private void SelfUpdate(string updateFile, string selfFile)
    {
        var stream = File.OpenRead(updateFile);

        var selfFileName = Path.GetFileName(selfFile);
        var directoryName = Path.GetDirectoryName(selfFile);
        var downloadedFilesPath = Path.GetDirectoryName(updateFile);
        if (string.IsNullOrEmpty(directoryName))
            return;

        string selfWithoutExt = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(selfFile));
        SaveStreamToFile(stream, selfWithoutExt + GetUpdateFilePath());

        string updateExeFile = selfWithoutExt + GetUpdateFilePath();
        string scriptFile = selfWithoutExt + GetScriptFilePath();

        var updateScript = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 
            string.Format(Resources.UpdateScript_Bat, selfFileName, updateExeFile, selfFile, updateExeFile, downloadedFilesPath) : 
            string.Format(Resources.UpdateScript_Shell, updateExeFile, selfFile, selfFileName, updateExeFile, downloadedFilesPath);

        StreamWriter streamWriter = new StreamWriter(File.Create(scriptFile));

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            streamWriter.NewLine = "\n";

        streamWriter.Write(updateScript);
        streamWriter.Close();

        ProcessStartInfo startInfo = new ProcessStartInfo(scriptFile)
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

    private string LookupBinaryFilePath(string path)
    {
        var binFileName = "synx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }

    private string GetUpdateFilePath()
    {
        var binFileName = "Update";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return binFileName;
    }

    private string GetScriptFilePath()
    {
        var scriptFileName = "Update.sh";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            scriptFileName = "Update.bat";

        return scriptFileName;
    }

    private class DownloadUpdateResult
    {
        public bool Success { get; init; }
        public string FlowSynxDownloadPath { get; init; } = string.Empty;
        public string FlowSynxCliDownloadPath { get; init; } = string.Empty;
    }
}

internal class GitHubTag
{
    public required string Name { get; set; }
}