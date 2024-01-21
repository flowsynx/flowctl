using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Storage;

internal class DeleteFileCommand : BaseCommand<DeleteFileCommandOptions, DeleteFileCommandOptionsHandler>
{
    public DeleteFileCommand() : base("deletefile", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "--path" }, "The path to get about") { IsRequired = true };

        AddOption(pathOption);
    }
}

internal class DeleteFileCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
}

internal class DeleteFileCommandOptionsHandler : ICommandOptionsHandler<DeleteFileCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public DeleteFileCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IEndpoint endpoint, IHttpRequestService httpRequestService)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
    }

    public async Task<int> HandleAsync(DeleteFileCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await CallApi(options, cancellationToken));
        return 0;
    }

    private async Task CallApi(DeleteFileCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "storage/deletefile";
            var request = new DeleteFileRequest { Path = options.Path };
            var result = await _httpRequestService.DeleteRequestAsync<DeleteFileRequest, Result<DeleteFileResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

            if (result is { Succeeded: false })
                _outputFormatter.WriteError(result.Messages);
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

public class DeleteFileRequest
{
    public required string Path { get; set; }
}

public class DeleteFileResponse
{

}