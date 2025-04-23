using FlowCtl.Core.Services.Authentication;
using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Login;

internal class LoginCommandOptionsHandler : ICommandOptionsHandler<LoginCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly IAuthenticationManager _authenticationManager;

    public LoginCommandOptionsHandler(IFlowCtlLogger flowCtlLogger,
        IAuthenticationManager authenticationManager)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _authenticationManager = authenticationManager ?? throw new ArgumentNullException(nameof(authenticationManager));
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
                    throw new InvalidOperationException(Resources.Commands_Login_BasicAuthentication_EnterUsernameAndPassword);

                _authenticationManager.LoginBasic(options.Username!, options.Password!);
                _flowCtlLogger.Write(Resources.Commands_Login_BasicAuthenticationLoggedSuccessfully);
            }
            else if (options.Bearer is true)
            {
                if (string.IsNullOrEmpty(options.Token))
                    throw new InvalidOperationException(Resources.Commands_Login_BearerAuthenticationEnterToken);

                _authenticationManager.LoginBearer(options.Token);
                _flowCtlLogger.Write(Resources.Commands_Login_BearerAuthenticationLoggedSuccessfully);
            }
            else if (options.OAuth is true)
            {
                if (string.IsNullOrWhiteSpace(options.Authority) || string.IsNullOrWhiteSpace(options.ClientId))
                    throw new InvalidOperationException(Resources.Commands_Login_OAuth2AuthenticationEnterAuthorityAndClientId);
                
                var result = await _authenticationManager.LoginOAuthAsync(options.Authority, options.ClientId, options.Scope);
                _flowCtlLogger.Write(Resources.Commands_Login_OAuth2AuthenticationLoggedSuccessfully);
            }
            else
            {
                throw new InvalidOperationException(Resources.Commands_Login_SpecifyAuthenticationType);
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