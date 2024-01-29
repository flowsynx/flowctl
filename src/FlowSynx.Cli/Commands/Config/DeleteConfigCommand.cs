using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Config;

internal class DeleteConfigCommand : BaseCommand<DeleteConfigCommandOptions, DeleteConfigCommandOptionsHandler>
{
    public DeleteConfigCommand() : base("delete", "Delete configuration section")
    {
        var nameOption = new Option<string>("--name", "The configuration section name") { IsRequired = true };

        AddOption(nameOption);
    }
}

internal class DeleteConfigCommandOptions : ICommandOptions
{
    public string Name { get; set; } = string.Empty;
}

internal class DeleteConfigCommandOptionsHandler : ICommandOptionsHandler<DeleteConfigCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public DeleteConfigCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
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

    public async Task<int> HandleAsync(DeleteConfigCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(DeleteConfigCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "config/delete";
            var request = new DeleteConfigRequest { Name = options.Name };
            var result = await _httpRequestService.DeleteRequestAsync<DeleteConfigRequest, Result<DeleteConfigResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

            if (result is { Succeeded: false })
                _outputFormatter.WriteError(result.Messages);
            else
                _outputFormatter.Write(result?.Data);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}

public class DeleteConfigRequest
{
    public required string Name { get; set; }
}

public class DeleteConfigResponse {}