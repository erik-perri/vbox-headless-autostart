using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace ServiceApp
{
    public partial class VBoxHeadlessAutoStart : ServiceBase
    {
        private readonly ILogger<VBoxHeadlessAutoStart> logger;

        public VBoxHeadlessAutoStart(ILogger<VBoxHeadlessAutoStart> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

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

            SetServiceState(NativeMethods.ServiceState.SERVICE_START_PENDING);

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
                    break;

                default:
                    base.OnCustomCommand(command);
                    break;
            }
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