using EnsureThat;
using FlowCtl.Core.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Responses;

namespace FlowCtl.Commands.Invoke;

internal class InvokeCommandOptionsHandler : ICommandOptionsHandler<InvokeCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;

    public InvokeCommandOptionsHandler(IFlowCtlLogger flowCtlLogger, 
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(flowCtlLogger, nameof(flowCtlLogger));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _flowCtlLogger = flowCtlLogger;
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
            switch (options.Method.ToLower())
            {
                case "read":
                case "compress":
                    GenerateOutput(await _flowSynxClient.InvokeMethod(httpMethod, options.Method, data, cancellationToken));
                    break;
                default:
                    var result = await _flowSynxClient.InvokeMethod<object?, object>(httpMethod, options.Method, data, cancellationToken);
                    GenerateOutput(result, options.Output);
                    break;
            }
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
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

    private void GenerateOutput(HttpResult<Result<object>> result, OutputType outputType)
    {
        if (result.StatusCode != 200)
            throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

        var payload = result.Payload;
        if (payload is { Succeeded: false })
        {
            _flowCtlLogger.WriteError(payload.Messages);
        }
        else
        {
            if (payload?.Data is not null)
                _flowCtlLogger.Write(payload.Data, outputType);
            else
                _flowCtlLogger.Write(payload?.Messages);
        }
    }

    private void GenerateOutput(HttpResult<byte[]> result)
    {
        if (result.StatusCode != 200)
            throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

        using (Stream stdout = Console.OpenStandardOutput())
        {
            stdout.Write(result.Payload, 0, result.Payload.Length);
        }
    }
}