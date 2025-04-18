﻿using FlowCtl.Core.Services.Logger;

namespace FlowCtl.Commands.Version;

internal class VersionCommandOptions : ICommandOptions
{
    public OutputType Output { get; set; } = OutputType.Json;
}