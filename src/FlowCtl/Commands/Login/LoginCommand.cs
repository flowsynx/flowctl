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

        var userameOption = new Option<string?>(new string[] { "-u", "--username" },
            description: Resources.Commands_Login_BasicAuthenticationUsernameOption);

        var passwordOption = new Option<string?>(new string[] { "-p", "--password" },
            description: Resources.Commands_Login_BasicAuthenticationPasswordOption);

        var tokenOption = new Option<string?>(new string[] { "-t", "--token" },
            description: Resources.Commands_Login_BearerAuthenticationTokenOption);

        AddOption(basicOption);
        AddOption(bearerOption);
        AddOption(userameOption);
        AddOption(passwordOption);
        AddOption(tokenOption);
    }
}