﻿namespace FlowCtl.Commands.Login;

internal class LoginCommandOptions : ICommandOptions
{
    public bool? Basic { get; set; } = false;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool? Bearer { get; set; } = false;
    public string? Token { get; set; }
}