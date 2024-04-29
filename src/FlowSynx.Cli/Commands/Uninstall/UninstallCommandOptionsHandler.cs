using System.Diagnostics;
using System.Runtime.InteropServices;
using EnsureThat;
using FlowSynx.Cli.Common;
using FlowSynx.Cli.Services.Abstracts;
using FlowSynx.Environment;

namespace FlowSynx.Cli.Commands.Uninstall;

internal class UninstallCommandOptionsHandler : ICommandOptionsHandler<UninstallCommandOptions>
{
    private readonly IOutputFormatter _outputFormatter;
    private readonly ISpinner _spinner;
    private readonly ILocation _location;
    private readonly IProcessHandler _processHandler;

    public UninstallCommandOptionsHandler(IOutputFormatter outputFormatter, ISpinner spinner, 
        ILocation location, IProcessHandler processHandler)
    {
        EnsureArg.IsNotNull(outputFormatter, nameof(outputFormatter));
        EnsureArg.IsNotNull(spinner, nameof(spinner));
        EnsureArg.IsNotNull(location, nameof(location));

        _outputFormatter = outputFormatter;
        _spinner = spinner;
        _location = location;
        _processHandler = processHandler;
    }

    public async Task<int> HandleAsync(UninstallCommandOptions options, CancellationToken cancellationToken)
    {
        await _spinner.DisplayLineSpinnerAsync(() => Execute(options));
        return 0;
    }

    private Task Execute(UninstallCommandOptions options)
    {
        try
        {
            _outputFormatter.Write("Beginning uninstalling...");

            if (_processHandler.IsRunning("flowsynx", "."))
            {
                if (options.Force)
                {
                    _processHandler.Terminate("flowsynx", ".");
                    _outputFormatter.Write("The FlowSynx system was stopped successfully.");
                }
                else
                {
                    _outputFormatter.Write("The FlowSynx engine is running. Please stop it by run the command: 'Synx stop', and try uninstall again.");
                    return Task.CompletedTask;
                }
            }

            if (_processHandler.IsRunning("dashboard", "."))
            {
                if (options.Force)
                {
                    _processHandler.Terminate("dashboard", ".");
                    _outputFormatter.Write("The FlowSynx dashboard was stopped successfully.");
                }
                else
                {
                    _outputFormatter.Write("The FlowSynx dashboard is running. Please stop it and try uninstall again.");
                    return Task.CompletedTask;
                }
            }
            
            if (Directory.Exists(_location.DefaultFlowSynxDirectoryName))
                Directory.Delete(_location.DefaultFlowSynxDirectoryName, true);

            SelfDestruction();
            _outputFormatter.Write("Uninstalling is done!");
        }
        catch (Exception e)
        {
            _outputFormatter.WriteError(e.Message);
        }
        return Task.CompletedTask;
    }

    private void SelfDestruction()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            const string scriptFile = "delete.bat";
            var strPath = Path.Combine(Directory.GetCurrentDirectory(), scriptFile);
            var strExe = new FileInfo(_location.LookupSynxBinaryFilePath(_location.RootLocation)).Name;
            var directoryName = Path.GetDirectoryName(strPath);

            var deleteScript = string.Format(Resources.DeleteScript_Bat, strExe, scriptFile);
            StreamWriter streamWriter = new(strPath);
            streamWriter.Write(deleteScript);
            streamWriter.Close();

            ProcessStartInfo startInfo = new(scriptFile)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = directoryName
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                _outputFormatter.WriteError(ex.Message);
                System.Environment.Exit(0);
            }
        }
        else
        {
            var strExe = new FileInfo(_location.LookupSynxBinaryFilePath(_location.RootLocation)).FullName;
            File.Delete(strExe);
        }
    }
}