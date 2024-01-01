using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Storage;

internal class CopyCommand : BaseCommand<CopyCommandOptions, CopyCommandOptionsHandler>
{
    public CopyCommand() : base("copy", "List of entities regarding specific path")
    {
        var sourcePathOption = new Option<string>(new[] { "--source-path" }, "The path to get about") { IsRequired = true };
        var destinationPathOption = new Option<string>(new[] { "--destination-path" }, "The path to get about") { IsRequired = true };
        var clearDestinationPathOption = new Option<bool?>(new[] { "--clear-destination-path" }, getDefaultValue: () => false, "The maximum number of results to return [default: off]");
        var overWriteDataOption = new Option<bool?>(new[] { "--overWrite-data" }, getDefaultValue: () => false, "Formatting CLI output");
        var includeOption = new Option<string?>(new[] { "--include" }, "Include entities matching pattern");
        var excludeOption = new Option<string?>(new[] { "--exclude" }, "Exclude entities matching pattern");
        var minAgeOption = new Option<string?>(new[] { "--min-age" }, "Filter entities older than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var maxAgeOption = new Option<string?>(new[] { "--max-age" }, "Filter entities younger than this in s or suffix ms|s|m|h|d|w|M|y [default: off]");
        var minSizeOption = new Option<string?>(new[] { "--min-size" }, "Filter entities bigger than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var maxSizeOption = new Option<string?>(new[] { "--max-size" }, "Filter entities smaller than this in KiB or suffix B|K|M|G|T|P [default: off]");
        var caseSensitiveOption = new Option<bool?>(new[] { "--case-sensitive" }, getDefaultValue: () => false, "Ignore or apply case sensitive in filters");
        var recurseOption = new Option<bool?>(new[] { "--recurse" }, getDefaultValue: () => false, "Apply recursion on filtering entities in the specified path");

        AddOption(sourcePathOption);
        AddOption(destinationPathOption);
        AddOption(clearDestinationPathOption);
        AddOption(overWriteDataOption);
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

internal class CopyCommandOptions : ICommandOptions
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
    public bool? ClearDestinationPath { get; set; } = false;
    public bool? OverWriteData { get; set; } = false;
    public string? Include { get; set; } = string.Empty;
    public string? Exclude { get; set; } = string.Empty;
    public string? MinAge { get; set; } = string.Empty;
    public string? MaxAge { get; set; } = string.Empty;
    public string? MinSize { get; set; } = string.Empty;
    public string? MaxSize { get; set; } = string.Empty;
    public bool? CaseSensitive { get; set; } = false;
    public bool? Recurse { get; set; } = false;
}

internal class CopyCommandOptionsHandler : ICommandOptionsHandler<CopyCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public CopyCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IEndpoint endpoint, IHttpRequestService httpRequestService)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));
        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
    }

    public async Task<int> HandleAsync(CopyCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await CallApi(options, cancellationToken));
        return 0;
    }

    private async Task CallApi(CopyCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "storage/copy";
            var request = new CopyRequest
            {
                SourcePath = options.SourcePath,
                DestinationPath = options.DestinationPath,
                Include = options.Include,
                Exclude = options.Exclude,
                MinAge = options.MinAge,
                MaxAge = options.MaxAge,
                MinSize = options.MinSize,
                MaxSize = options.MaxSize,
                CaseSensitive = options.CaseSensitive,
                Recurse = options.Recurse,
                ClearDestinationPath = options.ClearDestinationPath,
                OverWriteData = options.OverWriteData
            };
            var result = await _httpRequestService.PostAsync<CopyRequest, Result<CopyResponse?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", request, cancellationToken);

            if (!result.Succeeded)
                _outputFormatter.WriteError(result.Messages);
            else
            {
                if (result.Data is not null)
                    _outputFormatter.Write(result.Data);
                else
                    _outputFormatter.Write(result.Messages);
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}

public class CopyRequest
{
    public required string SourcePath { get; set; }
    public required string DestinationPath { get; set; }
    public string? Include { get; set; }
    public string? Exclude { get; set; }
    public string? MinAge { get; set; }
    public string? MaxAge { get; set; }
    public string? MinSize { get; set; }
    public string? MaxSize { get; set; }
    public bool? CaseSensitive { get; set; } = false;
    public bool? Recurse { get; set; } = false;
    public bool? ClearDestinationPath { get; set; } = false;
    public bool? OverWriteData { get; set; } = false;
}

public class CopyResponse
{

}
