using EnsureThat;
using FlowSynx.Abstractions;
using FlowSynx.Cli.Formatter;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Storage;

namespace FlowSynx.Cli.Commands.Storage.DeleteFile;

internal class DeleteFileCommandOptionsHandler : ICommandOptionsHandler<DeleteFileCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IFlowSynxClient _flowSynxClient;

    public DeleteFileCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(DeleteFileCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(DeleteFileCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Url))
                _flowSynxClient.ChangeConnection(options.Url);

            var request = new DeleteFileRequest { Path = options.Path };
            var result = await _flowSynxClient.DeleteFile(request, cancellationToken);
            
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