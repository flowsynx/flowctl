using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Requests.Logs;
using System.ComponentModel;

namespace FlowCtl.Commands.Logs;

internal class LogsCommandOptionsHandler : ICommandOptionsHandler<LogsCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IAuthenticationManager _authenticationManager;

    public LogsCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IAuthenticationManager authenticationManager)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _flowSynxClient = flowSynxClient ?? throw new ArgumentNullException(nameof(flowSynxClient));
        _authenticationManager = authenticationManager ?? throw new ArgumentNullException(nameof(authenticationManager));
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
            _authenticationManager.AuthenticateClient(_flowSynxClient);

            if (!string.IsNullOrEmpty(options.Address))
                _flowSynxClient.ChangeConnection(options.Address);

            var request = new LogsListRequest { 
                Level = options.Level, 
                FromDate = options.FromDate, 
                ToDate = options.ToDate, 
                Message = options.Message
            };
            var result = await _flowSynxClient.LogsList(request, cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.ErrorOccurredDuringProcessingRequest);

            var payload = result.Payload;
            if (payload is { Succeeded: false })
                _flowCtlLogger.WriteError(payload.Messages);
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

                _flowCtlLogger.Write(payload.Data, options.Output);
            }
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
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