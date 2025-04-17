using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Logout;

internal class LogoutCommandOptionsHandler : ICommandOptionsHandler<LogoutCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IAuthenticationManager _authenticationManager;

    public LogoutCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IAuthenticationManager authenticationManager)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _authenticationManager = authenticationManager ?? throw new ArgumentNullException(nameof(authenticationManager));
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