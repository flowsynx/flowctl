using EnsureThat;
using FlowCtl.Core.Logger;
using FlowCtl.Core.Services;

namespace FlowCtl.Commands.Uninstall;

internal class UninstallCommandOptionsHandler : ICommandOptionsHandler<UninstallCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;
    private readonly IProcessHandler _processHandler;

    public UninstallCommandOptionsHandler(IFlowCtlLogger flowCtlLogger, 
        ILocation location, IProcessHandler processHandler)
    {
        EnsureArg.IsNotNull(flowCtlLogger, nameof(flowCtlLogger));
        EnsureArg.IsNotNull(location, nameof(location));
        _flowCtlLogger = flowCtlLogger;
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
            _flowCtlLogger.Write(Resources.UninstallCommandBeginningUninstalling);

            if (_processHandler.IsRunning(_location.FlowSynxBinaryName, "."))
            {
                if (options.Force)
                {
                    _processHandler.Terminate(_location.FlowSynxBinaryName, ".");
                    _flowCtlLogger.Write(Resources.UninstallCommandFlowSynxStoppedSuccessfully);
                }
                else
                {
                    _flowCtlLogger.Write(Resources.UninstallCommandFlowSynxIsRunning);
                    return Task.CompletedTask;
                }
            }

            if (Directory.Exists(_location.DefaultFlowSynxDirectoryName))
                Directory.Delete(_location.DefaultFlowSynxDirectoryName, true);
            
            _flowCtlLogger.Write(Resources.UninstallCommandUninstallingIsDone);
        }
        catch (Exception e)
        {
            _flowCtlLogger.WriteError(e.Message);
        }
        return Task.CompletedTask;
    }
}