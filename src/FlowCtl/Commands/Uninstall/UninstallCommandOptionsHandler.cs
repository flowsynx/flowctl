using FlowCtl.Core.Services.Location;
using FlowCtl.Core.Services.Logger;
using FlowCtl.Core.Services.ProcessHost;

namespace FlowCtl.Commands.Uninstall;

internal class UninstallCommandOptionsHandler : ICommandOptionsHandler<UninstallCommandOptions>
{
    private readonly IFlowCtlLogger _flowCtlLogger;
    private readonly ILocation _location;
    private readonly IProcessHandler _processHandler;

    public UninstallCommandOptionsHandler(IFlowCtlLogger flowCtlLogger, 
        ILocation location, IProcessHandler processHandler)
    {
        _flowCtlLogger = flowCtlLogger ?? throw new ArgumentNullException(nameof(flowCtlLogger));
        _location = location ?? throw new ArgumentNullException(nameof(location));
        _processHandler = processHandler ?? throw new ArgumentNullException(nameof(processHandler));
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