using EnsureThat;
using FlowCtl.Extensions;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;

namespace FlowCtl.Commands.Invoke;

internal class InvokeCommandOptionsHandler : ICommandOptionsHandler<InvokeCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IFlowSynxClient _flowSynxClient;

    public InvokeCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner,
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(InvokeCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(InvokeCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var httpMethod = GetHttpMethod(options.Verb);

            string? jsonData;
            if (!string.IsNullOrEmpty(options.DataFile))
            {
                if (!File.Exists(options.DataFile))
                    throw new Exception($"Entered data file '{options.DataFile}' is not exist.");

                jsonData = await File.ReadAllTextAsync(options.DataFile, cancellationToken);
            }
            else
            {
                jsonData = options.Data;
            }

            var data = jsonData.JsonToObject();
            var result = await _flowSynxClient.InvokeMethod<object?, object>(httpMethod, options.Method, data, cancellationToken);
            
            if (result is { Succeeded: false })
            {
                _outputFormatter.WriteError(result.Messages);
            }
            else
            {
                if (result?.Data is not null)
                    _outputFormatter.Write(result.Data, options.Output);
                else
                    _outputFormatter.Write(result?.Messages);
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }

    private HttpMethod GetHttpMethod(Verb verb)
    {
        return verb switch
        {
            Verb.Get => HttpMethod.Get,
            Verb.Post => HttpMethod.Post,
            Verb.Put => HttpMethod.Put,
            Verb.Delete => HttpMethod.Delete,
            _ => throw new Exception("Entered verb is not valid!")
        };
    }
}