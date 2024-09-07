using EnsureThat;
using FlowCtl.Services.Abstracts;
using FlowSynx.Environment;

namespace FlowCtl.Commands.Uninstall;

internal class UninstallCommandOptionsHandler : ICommandOptionsHandler<UninstallCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ILocation _location;
    private readonly IProcessHandler _processHandler;

    public UninstallCommandOptionsHandler(IOutputFormatter outputFormatter, 
        ILocation location, IProcessHandler processHandler)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(location, nameof(location));
        _outputFormatter = outputFormatter;
        _location = location;
        _processHandler = processHandler;
    }

    public async Task<int> HandleAsync(UninstallCommandOptions options, CancellationToken cancellationToken)
    {
        await Execute(options);
        return 0;
    }

    private Task Execute(UninstallCommandOptions options)
    {
        try
        {
            _outputFormatter.Write(Resources.UninstallCommandBeginningUninstalling);

            if (_processHandler.IsRunning(_location.FlowSynxBinaryName, "."))
            {
                if (options.Force)
                {
                    _processHandler.Terminate(_location.FlowSynxBinaryName, ".");
                    _outputFormatter.Write(Resources.UninstallCommandFlowSynxStoppedSuccessfully);
                }
                else
                {
                    _outputFormatter.Write(Resources.UninstallCommandFlowSynxIsRunning);
                    return Task.CompletedTask;
                }
            }

            if (_processHandler.IsRunning(_location.DashboardBinaryName, "."))
            {
                if (options.Force)
                {
                    _processHandler.Terminate(_location.DashboardBinaryName, ".");
                    _outputFormatter.Write(Resources.UninstallCommandDashboardStoppedSuccessfully);
                }
                else
                {
                    _outputFormatter.Write(Resources.UninstallCommandDashboardIsRunning);
                    return Task.CompletedTask;
                }
            }
            
            if (Directory.Exists(_location.DefaultFlowSynxDirectoryName))
                Directory.Delete(_location.DefaultFlowSynxDirectoryName, true);
            
            _outputFormatter.Write(Resources.UninstallCommandUninstallingIsDone);
        }
        catch (Exception e)
        {
            _outputFormatter.WriteError(e.Message);
        }
        return Task.CompletedTask;
    }
}