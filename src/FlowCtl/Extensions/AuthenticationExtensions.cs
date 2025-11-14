using FlowCtl.Core.Services.Authentication;
using FlowSynx.Client;
using FlowSynx.Client.Authentication;

namespace FlowCtl.Extensions;

public static class AuthenticationExtensions
{
    public static void AuthenticateClient(this IAuthenticationManager authenticationManager, IFlowSynxClient flowSynxClient)
    {
        var authenticationData = authenticationManager.GetData();
        if (authenticationData is null || authenticationData.Type == AuthenticationType.None)
        {
            // No credentials or explicitly no-auth: leave client unauthenticated.
            return;
        }

        switch (authenticationData.Type)
        {
            case AuthenticationType.Basic:
            {
                var auth = new BasicAuthenticationStrategy(authenticationData.Username!, authenticationData.Password!);
                flowSynxClient.SetAuthenticationStrategy(auth);
                break;
            }
            case AuthenticationType.Bearer:
            {
                var auth = new BearerTokenAuthStrategy(authenticationData.AccessToken!);
                flowSynxClient.SetAuthenticationStrategy(auth);
                break;
            }
            default:
                // Unknown type: no auth
                break;
        }
    }
}