using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Storage;
using FlowSynx.IO;

namespace FlowCtl.Commands.Storage.Read;

internal class ReadCommandOptionsHandler : ICommandOptionsHandler<ReadCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IFlowSynxClient _flowSynxClient;

    public ReadCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(ReadCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
        return 0;
    }

    private async Task Execute(ReadCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var request = new ReadRequest { Path = options.Path };
            var result = await _flowSynxClient.Read(request, cancellationToken);

            var filePath = options.SaveTo;
            if (Directory.Exists(filePath))
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(options.Path)}";
                filePath = Path.Combine(options.SaveTo, fileName);
            }

            if (!File.Exists(filePath) || (File.Exists(filePath) && options.Overwrite is true))
            {
                result.WriteTo(filePath);
            }
            else
            {
                throw new Exception(string.Format(Resources.ReadCommandFileAlreadyExist, filePath));
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }
}