using System.Diagnostics;
using System.Runtime.InteropServices;
using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Environment;
using Microsoft.Extensions.Logging;

namespace FlowCtl.Commands.Dashboard;

internal class DashboardCommandOptionsHandler : ICommandOptionsHandler<DashboardCommandOptions>
{
    private readonly ILogger<DashboardCommandOptionsHandler> _logger;
    private readonly IOutputFormatter _outputFormatter;
    private readonly ILocation _location;
    private readonly IEndpoint _endpoint;

    public DashboardCommandOptionsHandler(ILogger<DashboardCommandOptionsHandler> logger, 
        IOutputFormatter outputFormatter, ILocation location,
        IEndpoint endpoint)
    {
        EnsureArg.IsNotNull(logger, nameof(logger));
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(location, nameof(location));
        EnsureArg.IsNotNull(endpoint, nameof(endpoint));

        _logger = logger;
        _outputFormatter = outputFormatter;
        _location = location;
        _endpoint = endpoint;
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
            var processStarted = process.Start();

            if (processStarted)
            {
                var dashboardUrl = _endpoint.FlowSynxDashboardHttpEndpoint();
                OpenUrl(dashboardUrl);
                _logger.LogInformation($"Dashboard with address '{dashboardUrl}' launched in the web browser.");
            }

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

        if (!string.IsNullOrEmpty(options.Address))
            argList.Add($"--address {options.Address}");

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

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch (Exception ex)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                _logger.LogWarning(ex.Message);
            }
        }
    }
}