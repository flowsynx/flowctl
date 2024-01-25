using System.Runtime.InteropServices;
using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Cli.Services;
using FlowSynx.Environment;
using FlowSynx.IO.Compression;
using FlowSynx.IO.Serialization;

namespace FlowSynx.Cli.Commands.Execute;

internal class UninstallCommand : BaseCommand<UninstallCommandOptions, UninstallCommandOptionsHandler>
{
    public UninstallCommand() : base("uninstall", "Uninstalling FlowSynx engine and cli")
    {
    }
}

internal class UninstallCommandOptions : ICommandOptions
{

}

internal class UninstallCommandOptionsHandler : ICommandOptionsHandler<UninstallCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IVersion _version;
    private readonly ILocation _location;
    private readonly IOperatingSystemInfo _operatingSystemInfo;
    private readonly IZipFile _zipFile;
    private readonly IGZipFile _gZipFile;
    private readonly IDeserializer _deserializer;

    public UninstallCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
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

    public async Task<int> HandleAsync(UninstallCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(UninstallCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _outputFormatter.Write("Beginning uninstalling...");
        }
        catch (Exception e)
        {
            _outputFormatter.WriteError(e.Message);
        }
    }
    
    private string LookupBinaryFilePath(string path)
    {
        var binFileName = "FlowSynx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }
}