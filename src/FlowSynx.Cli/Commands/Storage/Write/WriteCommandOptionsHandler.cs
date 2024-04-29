using EnsureThat;
using FlowSynx.Cli.Services;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Storage;
using FlowSynx.IO;

namespace FlowSynx.Cli.Commands.Storage.Write;

internal class WriteCommandOptionsHandler : ICommandOptionsHandler<WriteCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IFlowSynxClient _flowSynxClient;

    public WriteCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _flowSynxClient = flowSynxClient;
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
            if (!string.IsNullOrEmpty(options.Url))
                _flowSynxClient.ChangeConnection(options.Url);

            if (string.IsNullOrEmpty(options.Data) && !string.IsNullOrEmpty(options.FileToUpload))
            {
                if (!File.Exists(options.FileToUpload))
                    throw new Exception($"The file {options.FileToUpload} is not exist!");

                var fs = File.Open(options.FileToUpload, FileMode.Open);
                options.Data = fs.ConvertToBase64();
            }

            if (options.Data is null)
                throw new Exception("The content is empty. Please provide a Base64String data.");

            var request = new WriteRequest { Path = options.Path, Data = options.Data, Overwrite = options.Overwrite };
            var result = await _flowSynxClient.Write(request, cancellationToken);
            
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