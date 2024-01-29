using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Storage;

internal class MakeDirectoryCommand : BaseCommand<MakeDirectoryCommandOptions, MakeDirectoryCommandOptionsHandler>
{
    public MakeDirectoryCommand() : base("mkdir", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>("--path", "The path to get about") { IsRequired = true };

        AddOption(pathOption);
    }
}

internal class MakeDirectoryCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
}

internal class MakeDirectoryCommandOptionsHandler : ICommandOptionsHandler<MakeDirectoryCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public MakeDirectoryCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
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

    public async Task<int> HandleAsync(MakeDirectoryCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(MakeDirectoryCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "storage/mkdir";
            var request = new MakeDirectoryRequest { Path = options.Path };
            var result = await _httpRequestService.PostRequestAsync<MakeDirectoryRequest, Result<MakeDirectoryResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

            if (result is {Succeeded: false})
            {
                _outputFormatter.WriteError(result.Messages);
            }
            else
            {
                if (result?.Data is not null)
                    _outputFormatter.Write(result.Data);
                else
                    _outputFormatter.Write(result?.Messages);
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}

public class MakeDirectoryRequest
{
    public required string Path { get; set; }
}

public class MakeDirectoryResponse {}