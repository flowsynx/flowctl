using FlowCtl.Core.Services.Authentication;
using FlowSynx.Client;
using System.Security.Authentication;

namespace FlowCtl.Extensions;

public static class AuthenticationExtensions
{
    public static void AuthenticateClient(this IAuthenticationManager authenticationManager, IFlowSynxClient flowSynxClient)
    {
        if (!authenticationManager.IsLoggedIn)
            throw new AuthenticationException(Resources.AuthenticationExtensions_AuthenticationIsRequired);

        var authenticationData = authenticationManager.GetData();
        if (authenticationManager.IsBasicAuthenticationUsed)
            flowSynxClient.UseBasicAuth(authenticationData?.Username!, authenticationData?.Password!);
        else
            flowSynxClient.UseBearerToken(authenticationData?.AccessToken!);
    }
}