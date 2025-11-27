using FlowCtl.Core.Models.Configuration;
using FlowCtl.Core.Serialization;
using FlowCtl.Core.Services.Configuration;
using FlowCtl.Core.Services.Location;

namespace FlowCtl.Infrastructure.Services.Configuration;

public class AppSettingsService : IAppSettingsService
{
    private const string SettingsFileName = "appsettings.json";

    private readonly ILocation _location;
    private readonly IJsonSerializer _serializer;
    private readonly IJsonDeserializer _deserializer;

    public AppSettingsService(ILocation location, IJsonSerializer serializer, IJsonDeserializer deserializer)
    {
        _location = location ?? throw new ArgumentNullException(nameof(location));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
    }

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        var settingsPath = GetSettingsPath();
        if (!File.Exists(settingsPath))
            return CreateDefaultSettings();

        try
        {
            var content = await File.ReadAllTextAsync(settingsPath, cancellationToken);
            if (string.IsNullOrWhiteSpace(content))
                return CreateDefaultSettings();

            var settings = _deserializer.Deserialize<AppSettings>(content);
            Normalize(settings);
            return settings;
        }
        catch
        {
            return CreateDefaultSettings();
        }
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        var settingsPath = GetSettingsPath();
        var settingsDirectory = Path.GetDirectoryName(settingsPath);
        if (!string.IsNullOrEmpty(settingsDirectory))
            Directory.CreateDirectory(settingsDirectory);

        var formatted = _serializer.Serialize(settings, new JsonSerializationConfiguration { Indented = true });
        await File.WriteAllTextAsync(settingsPath, formatted, cancellationToken);
    }

    public string GetSettingsPath()
    {
        return Path.Combine(_location.RootLocation, SettingsFileName);
    }

    private AppSettings CreateDefaultSettings()
    {
        var defaultHostPath = Path.Combine(_location.DefaultFlowSynxDirectoryName, "data");
        return new AppSettings
        {
            DeploymentMode = DeploymentMode.Binary,
            Docker = new DockerSettings
            {
                ImageName = "flowsynx/flowsynx",
                Tag = string.Empty,
                ContainerName = "flowsynx-engine",
                Port = 6262,
                HostDataPath = defaultHostPath,
                ContainerDataPath = "/app/data"
            },
            Binary = new BinarySettings()
        };
    }

    private static void Normalize(AppSettings settings)
    {
        settings.Docker ??= new DockerSettings();
        settings.Binary ??= new BinarySettings();
    }
}
