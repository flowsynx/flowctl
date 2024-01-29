using System.CommandLine;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Abstractions;
using FlowSynx.Cli.Formatter;
using FlowSynx.IO;

namespace FlowSynx.Cli.Commands.Storage;

internal class WriteCommand : BaseCommand<WriteCommandOptions, WriteCommandOptionsHandler>
{
    public WriteCommand() : base("write", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>("--path", "The path to get about") { IsRequired = true };
        var dataOption = new Option<string?>("--data", "The path to get about");
        var fileToUploadOption = new Option<string?>("--file-to-upload", "The path to get about");

        AddOption(pathOption);
        AddOption(dataOption);
        AddOption(fileToUploadOption);
    }
}

internal class WriteCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string? Data { get; set; } = string.Empty;
    public string? FileToUpload { get; set; } = string.Empty;
}

internal class WriteCommandOptionsHandler : ICommandOptionsHandler<WriteCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public WriteCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
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

    public async Task<int> HandleAsync(WriteCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(WriteCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "storage/write";

            if (string.IsNullOrEmpty(options.Data) && !string.IsNullOrEmpty(options.FileToUpload))
            {
                if (!File.Exists(options.FileToUpload))
                    throw new Exception($"The file {options.FileToUpload} is not exist!");

                var fs = File.Open(options.FileToUpload, FileMode.Open);
                options.Data = fs.ConvertToBase64();
            }

            if (options.Data is null)
                throw new Exception("The content is empty. Please provide a Base64String data.");

            var request = new WriteRequest { Path = options.Path, Data = options.Data };
            var result = await _httpRequestService.PostRequestAsync<WriteRequest, Result<WriteResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

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

public class WriteRequest
{
    public string Path { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}

public class WriteResponse {}