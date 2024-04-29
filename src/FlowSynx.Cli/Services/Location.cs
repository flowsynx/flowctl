using EnsureThat;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace FlowSynx.Cli.Services;

public class Location : ILocation
{
    private readonly ILogger<Location> _logger;
    private readonly string? _rootLocation = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

    public Location(ILogger<Location> logger)
    {
        EnsureArg.IsNotNull(logger, nameof(logger));
        _logger = logger;

        if (_rootLocation == null)
        {
            logger.LogError("Base location not found");
            throw new Exception(Resources.FlowSynxLocationBaseLocationNotFound);
        }
    }

    public string RootLocation => GetRootLocation();

    public string UserProfilePath => System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

    public string DefaultFlowSynxDirectoryName => Path.Combine(UserProfilePath, ".flowsynx");

    public string DefaultFlowSynxBinaryDirectoryName => Path.Combine(DefaultFlowSynxDirectoryName, "bin");

    public string GetScriptFilePath()
    {
        var scriptFileName = "Update.sh";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            scriptFileName = "Update.bat";

        return scriptFileName;
    }

    public string GetUpdateFilePath()
    {
        var binFileName = "Update";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return binFileName;
    }

    public string LookupDashboardBinaryFilePath(string path)
    {
        var binFileName = "dashboard";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }

    public string LookupFlowSynxBinaryFilePath(string path)
    {
        var binFileName = "flowsynx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }

    public string LookupSynxBinaryFilePath(string path)
    {
        var binFileName = "synx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }

    #region MyRegion
    private string GetRootLocation()
    {
        if (_rootLocation is not null) return _rootLocation;
        _logger.LogError("Root location not found");
        throw new Exception(Resources.FlowSynxLocationRootLocationNotFound);
    }
    #endregion
}