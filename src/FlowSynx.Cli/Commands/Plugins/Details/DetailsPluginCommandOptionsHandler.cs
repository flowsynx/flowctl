using EnsureThat;
using FlowSynx.Abstractions;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.Net;

namespace FlowSynx.Cli.Commands.Plugins.Details;

internal class DetailsPluginCommandOptionsHandler : ICommandOptionsHandler<DetailsPluginCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public DetailsPluginCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IEndpoint endpoint, IHttpRequestService httpRequestService)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
    }

    public async Task<int> HandleAsync(DetailsPluginCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(DetailsPluginCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var relativeUrl = $"plugins/details/{options.Id}";
            var result = await _httpRequestService.GetRequestAsync<Result<PluginDetailsResponse?>>($"{_endpoint.FlowSynxHttpEndpoint()}/{relativeUrl}", cancellationToken);

            var payLoad = result.Payload;
            if (payLoad is { Succeeded: false })
                _outputFormatter.WriteError(payLoad.Messages);
            else
                _outputFormatter.Write(payLoad?.Data, options.Output);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}