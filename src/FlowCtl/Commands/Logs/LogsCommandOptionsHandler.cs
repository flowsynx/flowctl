using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Logs;
using System.ComponentModel;
using FlowSynx.Client.Responses.Logs;

namespace FlowCtl.Commands.Logs;

internal class LogsCommandOptionsHandler : ICommandOptionsHandler<LogsCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly IFlowSynxClient _flowSynxClient;

    public LogsCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner, 
        IFlowSynxClient flowSynxClient)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(flowSynxClient, nameof(flowSynxClient));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _flowSynxClient = flowSynxClient;
    }

    public async Task<int> HandleAsync(LogsCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(async () => await Execute(options, cancellationToken));
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
                Include = options.Include,
                Exclude = options.Exclude,
                CaseSensitive = options.CaseSensitive,
                MinAge = options.MinAge, 
                MaxAge = options.MaxAge, 
                Level = options.Level,
                Sorting = options.Sorting,
                MaxResults = options.MaxResults
            };
            var result = await _flowSynxClient.LogsList(request, cancellationToken);

            if (result is { Succeeded: false })
                _outputFormatter.WriteError(result.Messages);
            else
            {
                var filePath = options.ExportTo;
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (!File.Exists(filePath))
                        SaveToLogFile(result.Data, filePath);
                    else
                        throw new Exception(string.Format(Resources.ReadCommandFileAlreadyExist, filePath));
                }

                _outputFormatter.Write(result?.Data, options.Output);
            }
        }
        catch (Exception ex)
        {
            _outputFormatter.WriteError(ex.Message);
        }
    }

    private void SaveToLogFile(IEnumerable<LogsListResponse> reportData, string path)
    {
        var lines = new List<string>();
        IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(LogsListResponse)).OfType<PropertyDescriptor>();
        var header = string.Join(",", props.ToList().Select(x => x.Name));
        lines.Add(header);
        var valueLines = reportData.Select(row => string.Join(",", header.Split(',').Select(a => row.GetType().GetProperty(a)?.GetValue(row, null))));
        lines.AddRange(valueLines);
        File.WriteAllLines(path, lines.ToArray());
    }
}