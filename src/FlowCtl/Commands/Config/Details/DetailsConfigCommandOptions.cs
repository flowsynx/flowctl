﻿using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Config.Details;

internal class DetailsConfigCommandOptions : ICommandOptions
{
    public required string ConfigId { get; set; }
    public string? Address { get; set; } = string.Empty;
    public OutputType Output { get; set; } = OutputType.Json;
}