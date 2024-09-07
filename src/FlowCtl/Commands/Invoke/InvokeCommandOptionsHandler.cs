using EnsureThat;
using FlowCtl.Extensions;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;
using FlowSynx.Client.Responses;

namespace FlowCtl.Commands.Invoke;

internal class InvokeCommandOptionsHandler : ICommandOptionsHandler<InvokeCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IFlowSynxClient _flowSynxClient;

    public InvokeCommandOptionsHandler(IOutputFormatter outputFormatter, 
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _outputFormatter = outputFormatter;
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

            if (string.Equals(options.Method, "read", StringComparison.OrdinalIgnoreCase))
            {
                var result = await _flowSynxClient.InvokeMethod(httpMethod, options.Method, data, cancellationToken);
                GenerateOutput(result);
            }
            else
            {
                var result = await _flowSynxClient.InvokeMethod<object?, object>(httpMethod, options.Method, data, cancellationToken);
                GenerateOutput(result, options.Output);
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

    private void GenerateOutput(HttpResult<Result<object>> result, Output output)
    {
        if (result.StatusCode != 200)
            throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

        var payload = result.Payload;
        if (payload is { Succeeded: false })
        {
            _outputFormatter.WriteError(payload.Messages);
        }
        else
        {
            if (payload?.Data is not null)
                _outputFormatter.Write(payload.Data, output);
            else
                _outputFormatter.Write(payload?.Messages);
        }
    }

    private void GenerateOutput(HttpResult<Stream> result)
    {
        if (result.StatusCode != 200)
            throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

        using (var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true })
        {
            Console.SetOut(writer);
            using (var reader = new StreamReader(result.Payload))
            {
                var output = reader.ReadToEnd();
                writer.WriteLine(output);
            }
        }

        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
    }


}