using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Storage;

internal class DeleteCommand : BaseCommand<DeleteCommandOptions, DeleteCommandOptionsHandler>
{
    public DeleteCommand() : base("delete", "List of entities regarding specific path")
    {
        var pathOption = new Option<string>(new[] { "--path" }, "The path to get about") { IsRequired = true };
        var includeOption = new Option<string?>(new[] { "--include" }, "Include entities matching pattern");
        var excludeOption = new Option<string?>(new[] { "--exclude" }, "Exclude entities matching pattern");
        var minAgeOption = new Option<string?>(new[] { "--min-age" }, "Filter entities older than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var maxAgeOption = new Option<string?>(new[] { "--max-age" }, "Filter entities younger than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var minSizeOption = new Option<string?>(new[] { "--min-size" }, "Filter entities bigger than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var maxSizeOption = new Option<string?>(new[] { "--max-size" }, "Filter entities smaller than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var caseSensitiveOption = new Option<bool?>(new[] { "--case-sensitive" }, getDefaultValue: () => false, "Ignore or apply case sensitive in filters");
        var recurseOption = new Option<bool?>(new[] { "--recurse" }, getDefaultValue: () => false, "Apply recursion on filtering entities in the specified path");

        AddOption(pathOption);
        AddOption(includeOption);
        AddOption(excludeOption);
        AddOption(minAgeOption);
        AddOption(maxAgeOption);
        AddOption(minSizeOption);
        AddOption(maxSizeOption);
        AddOption(caseSensitiveOption);
        AddOption(recurseOption);
    }
}

internal class DeleteCommandOptions : ICommandOptions
{
    public string Path { get; set; } = string.Empty;
    public string? Include { get; set; } = string.Empty;
    public string? Exclude { get; set; } = string.Empty;
    public string? MinAge { get; set; } = string.Empty;
    public string? MaxAge { get; set; } = string.Empty;
    public string? MinSize { get; set; } = string.Empty;
    public string? MaxSize { get; set; } = string.Empty;
    public bool? CaseSensitive { get; set; } = false;
    public bool? Recurse { get; set; } = false;
}

internal class DeleteCommandOptionsHandler : ICommandOptionsHandler<DeleteCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public DeleteCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
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

    public async Task<int> HandleAsync(DeleteCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await CallApi(options, cancellationToken));
        return 0;
    }

    private async Task CallApi(DeleteCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "storage/delete";
            var request = new DeleteRequest
            {
                Path = options.Path,
                Include = options.Include,
                Exclude = options.Exclude,
                MinAge = options.MinAge,
                MaxAge = options.MaxAge,
                MinSize = options.MinSize,
                MaxSize = options.MaxSize,
                CaseSensitive = options.CaseSensitive,
                Recurse = options.Recurse
            };

            var result = await _httpRequestService.DeleteRequestAsync<DeleteRequest, Result<DeleteResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

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

public class DeleteRequest
{
    public required string Path { get; set; }
    public string? Include { get; set; }
    public string? Exclude { get; set; }
    public string? MinAge { get; set; }
    public string? MaxAge { get; set; }
    public string? MinSize { get; set; }
    public string? MaxSize { get; set; }
    public bool? CaseSensitive { get; set; } = false;
    public bool? Recurse { get; set; } = false;
}

public class DeleteResponse
{
    public string? Id { get; set; }
    public string? Kind { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? Path { get; set; } = string.Empty;
    public string? Size { get; set; }
    public string? MimeType { get; set; }
    public DateTimeOffset? ModifiedTime { get; set; }
}
