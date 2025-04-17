using FlowCtl.Core.Authentication;
using FlowSynx.Client;

namespace FlowCtl.Extensions;

public static class AuthenticationExtensions
{
    public static void AuthenticateClient(this IAuthenticationManager authenticationManager, IFlowSynxClient flowSynxClient)
    {
        if (!authenticationManager.IsLoggedIn)
            throw new Exception("Authentication has not been performed. Please use Login command first, and then try again");

        var authenticationData = authenticationManager.GetData();
        if (authenticationManager.IsBasicAuthenticationUsed)
            flowSynxClient.UseBasicAuth(authenticationData?.Username!, authenticationData?.Password!);
        else
            flowSynxClient.UseBearerToken(authenticationData?.AccessToken!);
    }
}