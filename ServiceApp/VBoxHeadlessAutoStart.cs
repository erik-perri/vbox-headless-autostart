using CommonLib.Configuration;
using CommonLib.Processes;
using CommonLib.VirtualMachine;
using Microsoft.Extensions.Logging;
using ServiceApp.Configuration;
using ServiceApp.Processes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace ServiceApp
{
    public partial class VBoxHeadlessAutoStart : ServiceBase
    {
        private readonly ILogger<VBoxHeadlessAutoStart> logger;
        private readonly UserProfileLocator profileLocator;
        private readonly XmlConfigurationReaderFactory readerFactory;
        private readonly IMachineLocator machineLocator;
        private readonly IProcessOutputFactory processOutputFactory;
        private readonly IMachineController machineController;

        public VBoxHeadlessAutoStart(
            ILogger<VBoxHeadlessAutoStart> logger,
            UserProfileLocator profileLocator,
            XmlConfigurationReaderFactory readerFactory,
            IProcessOutputFactory processOutputFactory,
            IMachineLocator machineLocator,
            IMachineController machineController
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.profileLocator = profileLocator ?? throw new ArgumentNullException(nameof(profileLocator));
            this.readerFactory = readerFactory ?? throw new ArgumentNullException(nameof(readerFactory));
            this.processOutputFactory = processOutputFactory ?? throw new ArgumentNullException(nameof(processOutputFactory));
            this.machineLocator = machineLocator ?? throw new ArgumentNullException(nameof(machineLocator));
            this.machineController = machineController ?? throw new ArgumentNullException(nameof(machineController));

            InitializeComponent();

            CanPauseAndContinue = false;

            // Since we start and stop VirtualBox with VBoxManage.exe the service shutdown event is too late.  By
            // that point process creation will fail with ERROR_SHUTDOWN_IN_PROGRESS.  To get around this we register
            // for pre-shutdown notification and stop there instead.
            AcceptPreShutdownCommand();
        }

        protected override void OnStart(string[] args)
        {
            logger.LogTrace("Service start requested");

            SetServiceState(NativeMethods.ServiceState.SERVICE_RUNNING);
        }

        protected override void OnStop()
        {
            logger.LogTrace("Service stop requested");

            SetServiceState(NativeMethods.ServiceState.SERVICE_STOPPED);
        }

        protected override void OnCustomCommand(int command)
        {
            logger.LogDebug("Entered OnCustomCommand({command})", command);

            switch (command)
            {
                case NativeMethods.SERVICE_CONTROL_PRESHUTDOWN:
                    logger.LogInformation("Pre-Shutdown detected");

                    SetServiceState(NativeMethods.ServiceState.SERVICE_STOP_PENDING, 10000);

                    StopMachines();

                    SetServiceState(NativeMethods.ServiceState.SERVICE_STOPPED);
                    break;

                default:
                    base.OnCustomCommand(command);
                    break;
            }
        }

        private void StopMachines()
        {
            foreach (var process in Process.GetProcessesByName("explorer"))
            {
                logger.LogDebug($"Found explorer process running with ID {process.Id}");

                try
                {
                    var configuration = FindConfigurationFromProcess(process);
                    if (configuration == null)
                    {
                        logger.LogDebug($"Configuration not found for user running process ID {process.Id}");
                        continue;
                    }

                    if (!((ImpersonatedProcessOutputFactory)processOutputFactory).ImpersonateUserFromProcess(process))
                    {
                        logger.LogDebug($"Failed to impersonate user running process ID {process.Id}");
                        continue;
                    }

                    RequestAdditionalTime(5000);

                    var machines = machineLocator.ListMachinesWithMetadata().Where(
                        m => configuration.Machines.Any(c => c.Uuid == m.Uuid)
                    );
                    foreach (var machine in machines)
                    {
                        if (!machine.IsPoweredOn)
                        {
                            logger.LogDebug($"Skipping power off due to state {new { machine.Uuid, machine.Name, machine.Metadata }}");
                            continue;
                        }

                        var machineConfiguration = configuration.Machines.First(c => c.Uuid == machine.Uuid);

                        if (machineConfiguration.SaveState)
                        {
                            logger.LogInformation($"Saving state {new { machine.Uuid, machine.Name }}");

                            RequestAdditionalTime(10000);

                            if (!machineController.SaveState(machine))
                            {
                                logger.LogError($"Failed to save state {new { machine.Uuid, machine.Name }}");
                            }
                        }
                        else
                        {
                            logger.LogInformation($"Powering off {new { machine.Uuid, machine.Name }}");

                            const int waitLimit = 90000;

                            if (!machineController.AcpiPowerOff(
                                machine,
                                waitLimit,
                                () =>
                                {
                                    logger.LogDebug($"Waiting for power off {new { machine.Uuid, machine.Name }}");
                                    RequestAdditionalTime(1000);
                                }
                            ))
                            {
                                logger.LogError($"Failed to power off {new { machine.Uuid, machine.Name }}");
                            }
                        }
                    }
                }
                catch (Exception e) when (
                    e is ArgumentException ||
                    e is InvalidOperationException ||
                    e is Win32Exception
                )
                {
                    logger.LogError(e, "Caught exception while stopping machine");
                }
            }
        }

        private AppConfiguration FindConfigurationFromProcess(Process process)
        {
            var profilePath = profileLocator.LocatePathFromProcess(process);
            if (profilePath == null)
            {
                logger.LogError($"Failed to locate profile path for process {process.Id}");
                return null;
            }

            var reader = readerFactory.CreateReader(profilePath);
            if (reader == null)
            {
                logger.LogDebug($"Failed to create configuration reader for user running process {process.Id}");
                return null;
            }

            var configuration = reader.ReadConfiguration();
            if (configuration == null)
            {
                logger.LogDebug($"Failed to load configuration for user running process {process.Id}");
                return null;
            }

            logger.LogDebug($"Loaded configuration {new { configuration.LogLevel, configuration.ShowKeepAwakeMenu, MachineCount = configuration.Machines.Count }}");

            return configuration;
        }

        private bool SetServiceState(NativeMethods.ServiceState state, int waitHint = 0)
        {
            var status = new NativeMethods.ServiceStatus() { dwCurrentState = state };
            if (waitHint > 0)
            {
                status.dwWaitHint = waitHint;
            }

            return NativeMethods.SetServiceStatus(ServiceHandle, ref status);
        }

        private void AcceptPreShutdownCommand()
        {
            try
            {
                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

                // acceptedCommands is used in .NET Framework, _acceptedCommands is used in .NET Core
                var acceptedCommands = typeof(ServiceBase).GetField("acceptedCommands", bindingFlags)
                                    ?? typeof(ServiceBase).GetField("_acceptedCommands", bindingFlags);

                if (acceptedCommands == null)
                {
                    logger.LogError("Cannot enable pre-shutdown notifications, acceptedCommands field not found");
                    return;
                }

                var newCommands = (int)acceptedCommands.GetValue(this) | NativeMethods.SERVICE_ACCEPT_PRESHUTDOWN;
                acceptedCommands.SetValue(this, newCommands);
            }
            catch (Exception e) when (
                e is ArgumentException ||
                e is NotSupportedException ||
                e is TargetException ||
                e is FieldAccessException
            )
            {
                logger.LogError(e, "Failed to adjust acceptedCommands");
            }
        }

        private static class NativeMethods
        {
            // https://docs.microsoft.com/en-us/windows/win32/api/winsvc/ns-winsvc-service_status
            public enum ServiceState
            {
                SERVICE_STOPPED = 0x00000001,
                SERVICE_START_PENDING = 0x00000002,
                SERVICE_STOP_PENDING = 0x00000003,
                SERVICE_RUNNING = 0x00000004,
                SERVICE_CONTINUE_PENDING = 0x00000005,
                SERVICE_PAUSE_PENDING = 0x00000006,
                SERVICE_PAUSED = 0x00000007,
            }

            // https://docs.microsoft.com/en-us/windows/win32/api/winsvc/ns-winsvc-service_status
            [StructLayout(LayoutKind.Sequential)]
            public struct ServiceStatus
            {
                public int dwServiceType;
                public ServiceState dwCurrentState;
                public int dwControlsAccepted;
                public int dwWin32ExitCode;
                public int dwServiceSpecificExitCode;
                public int dwCheckPoint;
                public int dwWaitHint;
            };

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

            // https://docs.microsoft.com/en-us/windows/win32/api/winsvc/ns-winsvc-service_status_process
            public const int SERVICE_ACCEPT_PRESHUTDOWN = 0x00000100;

            // https://docs.microsoft.com/en-us/windows/win32/api/winsvc/nc-winsvc-lphandler_function_ex
            public const int SERVICE_CONTROL_PRESHUTDOWN = 0x0000000F;
        }
    }
}