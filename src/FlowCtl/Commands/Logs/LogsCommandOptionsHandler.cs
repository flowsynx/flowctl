using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Logs;
using System.ComponentModel;

namespace FlowCtl.Commands.Logs;

internal class LogsCommandOptionsHandler : ICommandOptionsHandler<LogsCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly IFlowSynxClient _flowSynxClient;

    public LogsCommandOptionsHandler(IOutputFormatter outputFormatter,
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));
        _outputFormatter = outputFormatter;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(LogsCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(LogsCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var request = new LogsListRequest
            {
                Fields = options.Fields,
                Filter = options.Filter,
                CaseSensitive = options.CaseSensitive,
                Sort = options.Sort,
                Limit = options.Limit
            };
            var result = await _flowSynxClient.LogsList(request, cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

            var payload = result.Payload;
            if (payload is { Succeeded: false })
                _outputFormatter.WriteError(payload.Messages);
            else
            {
                var filePath = options.ExportTo;
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (!File.Exists(filePath))
                        SaveToLogFile(payload.Data, filePath);
                    else
                        throw new Exception(string.Format(Resources.ReadCommandFileAlreadyExist, filePath));
                }

                _outputFormatter.Write(payload.Data, options.Output);
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }

    private void SaveToLogFile(IEnumerable<object> reportData, string path)
    {
        var lines = new List<string>();
        IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(object)).OfType<PropertyDescriptor>();
        var header = string.Join(",", props.ToList().Select(x => x.Name));
        lines.Add(header);
        var valueLines = reportData.Select(row => string.Join(",", header.Split(',').Select(a => row.GetType().GetProperty(a)?.GetValue(row, null))));
        lines.AddRange(valueLines);
        File.WriteAllLines(path, lines.ToArray());
    }
}