using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Client;
using FlowSynx.Environment;

namespace FlowSynx.Cli.Commands.Version;

internal class VersionCommandOptionsHandler : ICommandOptionsHandler<VersionCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IVersion _version;

    public VersionCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IFlowSynxClient flowSynxClient, IVersion version)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        EnsureArg.IsNotNull(version, nameof(version));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _flowSynxClient = flowSynxClient;
        _version = version;
    }

    public async Task<int> HandleAsync(VersionCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(VersionCommandOptions options, CancellationToken cancellationToken)
    {
        var cliVersion = _version.Version;
        try
        {
            if (options.Full is null or false)
            {
                var version = new { Cli = cliVersion };
                _outputFormatter.Write(version, options.Output);
                return;
            }

            if (!string.IsNullOrEmpty(options.Url))
                _flowSynxClient.ChangeConnection(options.Url);
            
            var result = await _flowSynxClient.Version(cancellationToken);
            if (result is { Succeeded: false })
            {
                _outputFormatter.WriteError(result.Messages);
            }
            else
            {
                if (result?.Data != null)
                {
                    var versionResponse = new VersionResponse
                    {
                        Cli = cliVersion,
                        FlowSynx = result.Data.FlowSynx,
                        OSVersion = result.Data.OSVersion,
                        OSArchitecture = result.Data.OSArchitecture,
                        OSType = result.Data.OSType,

                    };
                    _outputFormatter.Write(versionResponse, options.Output);
                }
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}