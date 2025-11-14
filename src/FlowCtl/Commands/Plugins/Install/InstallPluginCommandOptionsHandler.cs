using System.Globalization;
using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Extensions;
using FlowSynx.Client;
using FlowSynx.Client.Messages.Requests.Plugins;

namespace FlowCtl.Commands.Plugins.Install;

internal class InstallPluginCommandOptionsHandler : ICommandOptionsHandler<InstallPluginCommandOptions>
{
    private const string LatestPluginVersion = "latest"; // FlowSynx resolves this marker to the newest plugin release.

    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IFlowSynxClient _flowSynxClient;
    private readonly IAuthenticationManager _authenticationManager;

    public InstallPluginCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IFlowSynxClient flowSynxClient, IAuthenticationManager authenticationManager)
    {
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        ArgumentNullException.ThrowIfNull(flowSynxClient);
        ArgumentNullException.ThrowIfNull(flowCtlLogger);
        _flowCtlLogger = flowCtlLogger;
        _flowSynxClient = flowSynxClient;
        _authenticationManager = authenticationManager;
    }

    public async Task<int> HandleAsync(InstallPluginCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(InstallPluginCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _authenticationManager.AuthenticateClient(_flowSynxClient);

            if (!string.IsNullOrEmpty(options.Address))
            {
                var connection = new FlowSynxClientConnection(options.Address);
                _flowSynxClient.SetConnection(connection);
            }

            var resolvedVersion = ResolvePluginVersion(options.Version);
            if (string.IsNullOrWhiteSpace(options.Version))
            {
                _flowCtlLogger.Write(string.Format(CultureInfo.InvariantCulture,
                    Resources.Commands_Plugins_Install_DefaultVersionInfo, resolvedVersion));
            }

            var request = new InstallPluginRequest { Type = options.Type, Version = resolvedVersion };
            var result = await _flowSynxClient.Plugins.InstallAsync(request, cancellationToken);

            if (result.StatusCode != 200)
                throw new Exception(Resources.Commands_Error_DuringProcessingRequest);

            var payload = result.Payload;
            if (payload is { Succeeded: false })
                _flowCtlLogger.WriteError(payload.Messages);
            else
                _flowCtlLogger.Write(payload.Data);
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }

    /// <summary>
    /// Normalizes the requested version by falling back to the FlowSynx "latest" tag.
    /// </summary>
    private static string ResolvePluginVersion(string? requestedVersion) =>
        string.IsNullOrWhiteSpace(requestedVersion) ? LatestPluginVersion : requestedVersion;
}
