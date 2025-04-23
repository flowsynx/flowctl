using System.CommandLine;

namespace FlowCtl.Commands.Login;

internal class LoginCommand : BaseCommand<LoginCommandOptions, LoginCommandOptionsHandler>
{
    public LoginCommand() : base("login", Resources.Commands_Login_Description)
    {
        var basicOption = new Option<bool?>("--basic", 
            getDefaultValue: () => false, 
            description: Resources.Commands_Login_BasicAuthenticationOption);

        var bearerOption = new Option<bool?>("--bearer",
            getDefaultValue: () => false,
            description: Resources.Commands_Login_BearerAuthenticationOption);

        var oAuthOption = new Option<bool?>("--oauth",
            getDefaultValue: () => false,
            description: Resources.Commands_Login_OAuth2AuthenticationOption);

        var userameOption = new Option<string?>(new string[] { "-u", "--username" },
            description: Resources.Commands_Login_BasicAuthenticationUsernameOption);

        var passwordOption = new Option<string?>(new string[] { "-p", "--password" },
            description: Resources.Commands_Login_BasicAuthenticationPasswordOption);

        var tokenOption = new Option<string?>(new string[] { "-t", "--token" },
            description: Resources.Commands_Login_BearerAuthenticationTokenOption);

        var authorityOption = new Option<string?>(new string[] { "-a", "--authority" },
            description: Resources.Commands_Login_OAuth2AuthenticationAuthorityUrlOption);

        var clientIdOption = new Option<string?>(new string[] { "-c", "--client-id" },
            description: Resources.Commands_Login_OAuth2AuthenticationClientIdOption);

        var scopeOption = new Option<string?>(new string[] { "-s", "--scope" },
            description: Resources.Commands_Login_OAuth2AuthenticationScopeOption);


        AddOption(basicOption);
        AddOption(bearerOption);
        AddOption(oAuthOption);
        AddOption(userameOption);
        AddOption(passwordOption);
        AddOption(tokenOption);
        AddOption(authorityOption);
        AddOption(clientIdOption);
        AddOption(scopeOption);
    }
}