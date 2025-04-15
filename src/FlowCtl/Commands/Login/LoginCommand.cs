using System.CommandLine;

namespace FlowCtl.Commands.Login;

internal class LoginCommand : BaseCommand<LoginCommandOptions, LoginCommandOptionsHandler>
{
    public LoginCommand() : base("login", Resources.HealthCommandDescription)
    {
        var basicOption = new Option<bool?>("--basic", 
            getDefaultValue: () => false, 
            description: "Login using basic authentication");

        var userameOption = new Option<string?>(new string[] { "-u", "--username" },
            description: "User name for basic authentication");

        var passwordOption = new Option<string?>(new string[] { "-p", "--password" },
            description: "User name for basic authentication");

        var bearerOption = new Option<bool?>("--bearer", 
            getDefaultValue: () => false, 
            description: "Login using bearer authentication");

        var tokenOption = new Option<string?>(new string[] { "-t", "--token" },
            description: "Token for bearer authentication");

        AddOption(basicOption);
        AddOption(userameOption);
        AddOption(passwordOption);
        AddOption(bearerOption);
        AddOption(tokenOption);
    }
}