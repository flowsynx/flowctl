using EnsureThat;
using Microsoft.Extensions.Logging;

namespace FlowSynx.Cli;

public interface ILocation
{
    public string RootLocation { get; }
}

public class CliLocation : ILocation
{
    private readonly ILogger<CliLocation> _logger;
    private readonly string? _rootLocation = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

    public CliLocation(ILogger<CliLocation> logger)
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

    #region MyRegion
    private string GetRootLocation()
    {
        if (_rootLocation is not null) return _rootLocation;
        _logger.LogError("Root location not found");
        throw new Exception(Resources.FlowSynxLocationRootLocationNotFound);
    }
    #endregion
}
