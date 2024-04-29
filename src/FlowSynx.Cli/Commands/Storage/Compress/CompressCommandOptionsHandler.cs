using EnsureThat;
using FlowSynx.Cli.Common;
using FlowSynx.Cli.Services;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Storage;
using FlowSynx.IO;

namespace FlowSynx.Cli.Commands.Storage.Compress;

internal class CompressCommandOptionsHandler : ICommandOptionsHandler<CompressCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IFlowSynxClient _flowSynxClient;

    public CompressCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(CompressCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(CompressCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Url))
                _flowSynxClient.ChangeConnection(options.Url);

            var request = new CompressRequest()
            {
                Path = options.Path,
                Kind = options.Kind.ToString(),
                Include = options.Include,
                Exclude = options.Exclude,
                MinAge = options.MinAge,
                MaxAge = options.MaxAge,
                MinSize = options.MinSize,
                MaxSize = options.MaxSize,
                CaseSensitive = options.CaseSensitive,
                Recurse = options.Recurse,
                MaxResults = options.MaxResults,
                Hashing = options.Hashing,
                CompressType = options.CompressType.ToString()
            };

            var result = await _flowSynxClient.Compress(request, cancellationToken);

            if (!result.Succeeded)
            {
                _outputFormatter.WriteError(result.Messages);
                return;
            }

            if (result.Data.Content == null)
            {
                _outputFormatter.WriteError("No data received!");
                return;
            }

            var filePath = options.SaveTo;
            if (Directory.Exists(filePath))
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(options.Path)}";
                filePath = Path.Combine(options.SaveTo, fileName);
            }

            if (!File.Exists(filePath) || (File.Exists(filePath) && options.Overwrite is true))
            {
                result.Data.Content.WriteTo(filePath);
                _outputFormatter.Write($"Data saved to the '{filePath}' successfully.");
            }
            else
            {
                throw new Exception($"File '{filePath}' is already exist!");
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}