using FlowCtl.Core.Models.Configuration;

namespace FlowCtl.Core.Services.Configuration;

public interface IAppSettingsService
{
    /// <summary>
    /// Reads application settings from disk or returns defaults when the file is missing or invalid.
    /// </summary>
    Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists application settings to disk.
    /// </summary>
    Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the expected file path for the settings file.
    /// </summary>
    string GetSettingsPath();
}
