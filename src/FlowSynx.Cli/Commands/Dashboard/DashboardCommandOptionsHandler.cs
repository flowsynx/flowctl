using System.Diagnostics;
using EnsureThat;
using FlowSynx.Cli.Common;
using FlowSynx.Cli.Services;

namespace FlowSynx.Cli.Commands.Dashboard;

internal class DashboardCommandOptionsHandler : ICommandOptionsHandler<DashboardCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ILocation _location;

    public DashboardCommandOptionsHandler(IOutputFormatter outputFormatter, ILocation location)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(location, nameof(location));

        _outputFormatter = outputFormatter;
        _location = location;
    }

    public async Task<int> HandleAsync(DashboardCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options);
        return 0;
    }

    private Task Execute(DashboardCommandOptions options)
    {
        try
        {
            var dashboardPath = Path.Combine(_location.DefaultFlowSynxBinaryDirectoryName, "dashboard");
            var dashboardBinaryFile = _location.LookupDashboardBinaryFilePath(dashboardPath);
            if (!Path.Exists(dashboardBinaryFile))
            {
                _outputFormatter.WriteError(Resources.FlowSynxEngineIsNotInstalled);
                return Task.CompletedTask;
            }

            var startInfo = new ProcessStartInfo(dashboardBinaryFile)
            {
                Arguments = GetArgumentStr(options),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = dashboardPath
            };

            var process = new Process { StartInfo = startInfo };
            process.OutputDataReceived += OutputDataHandler;
            process.ErrorDataReceived += ErrorDataHandler;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process?.WaitForExit();
        }
        catch (Exception e)
        {
            _outputFormatter.WriteError(e.Message);
        }
        return Task.CompletedTask;
    }

    private string GetArgumentStr(DashboardCommandOptions options)
    {
        var argList = new List<string>();

        if (!string.IsNullOrEmpty(options.Url))
            argList.Add($"--url {options.Url}");

        return argList.Count == 0 ? string.Empty : string.Join(' ', argList);
    }

    private void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (outLine.Data != null) _outputFormatter.Write(outLine.Data);
    }

    private void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (outLine.Data != null) _outputFormatter.WriteError(outLine.Data);
    }
}