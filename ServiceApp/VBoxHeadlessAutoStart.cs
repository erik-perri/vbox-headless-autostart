using Microsoft.Extensions.Logging;
using System;
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

        private bool SetServiceState(NativeMethods.ServiceState state, int waitHint = 0)
        {
            var status = new NativeMethods.ServiceStatus() { dwCurrentState = state };
            if (waitHint > 0)
            {
                status.dwWaitHint = waitHint;
            }

            return NativeMethods.SetServiceStatus(ServiceHandle, ref status);
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
        }
    }
}