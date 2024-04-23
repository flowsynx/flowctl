using EnsureThat;
using FlowSynx.Cli.Formatter;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Storage;

namespace FlowSynx.Cli.Commands.Storage.Copy;

internal class CopyCommandOptionsHandler : ICommandOptionsHandler<CopyCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IFlowSynxClient _flowSynxClient;

    public CopyCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(CopyCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(CopyCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Url))
                _flowSynxClient.ChangeConnection(options.Url);

            var request = new CopyRequest()
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

            var result = await _flowSynxClient.Copy(request, cancellationToken);
            
            if (result is { Succeeded: false })
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