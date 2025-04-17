using EnsureThat;
using FlowCtl.Core.Authentication;
using FlowCtl.Core.Logger;

namespace FlowCtl.Commands.Login;

internal class LoginCommandOptionsHandler : ICommandOptionsHandler<LoginCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IAuthenticationManager _authenticationManager;

    public LoginCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IAuthenticationManager authenticationManager)
    {
        EnsureArg.IsNotNull(flowCtlLogger, nameof(flowCtlLogger));
        EnsureArg.IsNotNull(authenticationManager, nameof(authenticationManager));
        _authenticationManager = authenticationManager;
        _flowCtlLogger = flowCtlLogger;
    }

    public async Task<int> HandleAsync(LoginCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options, cancellationToken);
        return 0;
    }

    private async Task Execute(LoginCommandOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (options.Basic is true)
            {
                if (string.IsNullOrEmpty(options.Username) || string.IsNullOrEmpty(options.Password))
                    throw new InvalidOperationException("Please enter Username and Password!");

                _authenticationManager.LoginBasic(options.Username!, options.Password!);
                _flowCtlLogger.Write("Logged in with Basic Auth.");
            }
            else if (options.Bearer is true)
            {
                if (string.IsNullOrEmpty(options.Token))
                    throw new InvalidOperationException("Please enter token!");

                _authenticationManager.LoginBearer(options.Token);
                _flowCtlLogger.Write("Logged in with Bearer Token.");
            }
            else if (options.OAuth is true)
            {
                if (string.IsNullOrWhiteSpace(options.Authority) || string.IsNullOrWhiteSpace(options.ClientId))
                    throw new InvalidOperationException("Please provide --authority and --client-id for OAuth login.");
                
                var result = await _authenticationManager.LoginOAuthAsync(options.Authority, options.ClientId, options.Scope);
                _flowCtlLogger.Write("Logged in with OAuth.");
            }
            else
            {
                throw new InvalidOperationException("Please specify one of: --basic, --bearer, or --oauth");
            }

            return;
        }
        catch (Exception ex)
        {
            _flowCtlLogger.WriteError(ex.Message);
            return;
        }
    }
}