using System.CommandLine;

namespace FlowCtl.Commands.Login;

internal class LoginCommand : BaseCommand<LoginCommandOptions, LoginCommandOptionsHandler>
{
    public LoginCommand() : base("login", Resources.HealthCommandDescription)
    {
        var basicOption = new Option<bool?>("--basic", 
            getDefaultValue: () => false, 
            description: "Login using basic authentication");

        var bearerOption = new Option<bool?>("--bearer",
            getDefaultValue: () => false,
            description: "Login using bearer authentication");

        var oAuthOption = new Option<bool?>("--oauth",
            getDefaultValue: () => false,
            description: "Login using OAuth2 via browser");

        var userameOption = new Option<string?>(new string[] { "-u", "--username" },
            description: "User name for basic authentication");

        var passwordOption = new Option<string?>(new string[] { "-p", "--password" },
            description: "Password for basic authentication");

        var tokenOption = new Option<string?>(new string[] { "-t", "--token" },
            description: "Token for bearer authentication");

        var authorityOption = new Option<string?>(new string[] { "-a", "--authority" },
            description: "OAuth2 authority URL");

        var clientIdOption = new Option<string?>(new string[] { "-c", "--client-id" },
            description: "OAuth2 client ID");

        var scopeOption = new Option<string?>(new string[] { "-s", "--scope" },
            description: "Openid profile email, OAuth2 scopes");


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