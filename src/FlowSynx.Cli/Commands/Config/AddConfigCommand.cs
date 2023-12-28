using System.CommandLine;
using FlowSynx.Abstractions;
using FlowSynx.Environment;
using FlowSynx.IO.Serialization;
using FlowSynx.Net;
using EnsureThat;
using FlowSynx.Cli.Formatter;

namespace FlowSynx.Cli.Commands.Config;

internal class AddConfigCommand : BaseCommand<AddConfigCommandOptions, AddConfigCommandOptionsHandler>
{
    public AddConfigCommand() : base("add", "Add configuration section")
    {
        var nameOption = new Option<string>(new[] { "-n", "--name" }, "The path to get about") { IsRequired = true };
        var typeOption = new Option<string>(new[] { "-t", "--type" }, "The path to get about") { IsRequired = true };

        AddOption(nameOption);
        AddOption(typeOption);
    }
}

internal class AddConfigCommandOptions : ICommandOptions
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

internal class AddConfigCommandOptionsHandler : ICommandOptionsHandler<AddConfigCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IEndpoint _endpoint;
    private readonly IHttpRequestService _httpRequestService;

    public AddConfigCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner, 
        IEndpoint endpoint, IHttpRequestService httpRequestService, ISerializer serializer)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));
        EnsureArg.IsNotNull(httpRequestService, nameof(httpRequestService));
        EnsureArg.IsNotNull(serializer, nameof(serializer));
        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _endpoint = endpoint;
        _httpRequestService = httpRequestService;
    }

    public async Task<int> HandleAsync(AddConfigCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await CallApi(options, cancellationToken));
        return 0;
    }

    private async Task CallApi(AddConfigCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            const string relativeUrl = "config/add";
            var result = await _httpRequestService.PostAsync<AddConfigCommandOptions, Result<object?>>($"{_endpoint.GetDefaultHttpEndpoint()}/{relativeUrl}", options, cancellationToken);

            if (!result.Succeeded)
                _outputFormatter.WriteError(result.Messages);
            else
                _outputFormatter.Write(result.Data ?? result.Messages);
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}