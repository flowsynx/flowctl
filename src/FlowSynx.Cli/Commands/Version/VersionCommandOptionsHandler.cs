using EnsureThat;
using FlowSynx.Abstractions;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.Net;

namespace FlowSynx.Cli.Commands.Version;

internal class VersionCommandOptionsHandler : ICommandOptionsHandler<VersionCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;
    private readonly IVersion _version;

    public VersionCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IEndpoint endpoint, IHttpRequestService httpRequestService, IVersion version)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));
        EnsureArg.IsNotNull(version, nameof(version));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
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

            const string relativeUrl = "version";
            var result = await _httpRequestService.GetRequestAsync<Result<VersionResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", cancellationToken);

            if (result is { Succeeded: false })
            {
                _outputFormatter.WriteError(result.Messages);
            }
            else
            {
                if (result?.Data != null)
                {
                    result.Data.Cli = cliVersion;
                    _outputFormatter.Write(result.Data, options.Output);
                }
            }
        }
        catch
        {
            dynamic version = options.Full is null or false ? new { Cli = cliVersion } : new { Cli = cliVersion, FlowSynx = "N/A" };
            _outputFormatter.Write(version, options.Output);
        }
    }
}