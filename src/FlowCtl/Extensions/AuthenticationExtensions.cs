using FlowCtl.Core.Services.Authentication;
using FlowSynx.Client;
using FlowSynx.Client.Authentication;
using System.Security.Authentication;

namespace FlowCtl.Extensions;

public static class AuthenticationExtensions
{
    public static void AuthenticateClient(this IAuthenticationManager authenticationManager, IFlowSynxClient flowSynxClient)
    {
        if (!authenticationManager.IsLoggedIn)
            throw new AuthenticationException(Resources.AuthenticationExtensions_AuthenticationIsRequired);

        var authenticationData = authenticationManager.GetData();

        IAuthenticationStrategy authenticationStrategy;
        if (authenticationManager.IsBasicAuthenticationUsed)
            authenticationStrategy = new BasicAuthenticationStrategy(authenticationData?.Username!, authenticationData?.Password!);
        else
            authenticationStrategy = new BearerTokenAuthStrategy(authenticationData?.AccessToken!);

        flowSynxClient.SetAuthenticationStrategy(authenticationStrategy);
    }
}