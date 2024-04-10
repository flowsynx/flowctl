using EnsureThat;
using FlowSynx.Abstractions;
using FlowSynx.Cli.Formatter;
using FlowSynx.Environment;
using FlowSynx.Net;

namespace FlowSynx.Cli.Commands.Storage.List;

internal class ListCommandOptionsHandler : ICommandOptionsHandler<ListCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public ListCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
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

    public async Task<int> HandleAsync(ListCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(ListCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "storage/list";
            var request = new ListRequest
            {
                Path = options.Path,
                Kind = options.Kind,
                Include = options.Include,
                Exclude = options.Exclude,
                MinAge = options.MinAge,
                MaxAge = options.MaxAge,
                MinSize = options.MinSize,
                MaxSize = options.MaxSize,
                CaseSensitive = options.CaseSensitive,
                Recurse = options.Recurse,
                Full = options.Full,
                Sorting = options.Sorting,
                MaxResults = options.MaxResults
            };

            var result = await _httpRequestService.PostRequestAsync<ListRequest, Result<List<ListResponse>?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

            var payLoad = result.Payload;
            if (payLoad is { Succeeded: false })
            {
                _outputFormatter.WriteError(payLoad.Messages);
            }
            else
            {
                if (payLoad?.Data is not null)
                    _outputFormatter.Write(payLoad.Data, options.Output);
                else
                    _outputFormatter.Write(payLoad?.Messages);
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}