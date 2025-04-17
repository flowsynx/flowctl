using EnsureThat;
using FlowCtl.Core.Authentication;
using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Logout;

internal class LogoutCommandOptionsHandler : ICommandOptionsHandler<LogoutCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IAuthenticationManager _authenticationManager;

    public LogoutCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IAuthenticationManager authenticationManager)
    {
        EnsureArg.IsNotNull(flowCtlLogger, nameof(flowCtlLogger));
        EnsureArg.IsNotNull(authenticationManager, nameof(authenticationManager));
        _authenticationManager = authenticationManager;
        _flowCtlLogger = flowCtlLogger;
    }

    public Task<int> HandleAsync(LogoutCommandOptions options, CancellationToken cancellationToken)
    {
        Execute(options, cancellationToken);
        return Task.FromResult(0);
    }

    private void Execute(LogoutCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            _authenticationManager.Logout();
            _flowCtlLogger.Write("Logout successfully.");
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
        }
    }
}