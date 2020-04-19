using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using TrayApp.State;
using TrayApp.VirtualMachine;

namespace TrayApp.Shutdown
{
    public class ShutdownLocker
    {
        private readonly ILogger<ShutdownLocker> logger;
        private readonly AppState appState;

        public ShutdownLocker(
            ILogger<ShutdownLocker> logger,
            AppState appState
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));

            // Tell Windows we want highest shutdown priority
            NativeMethods.SetProcessShutdownParameters(NativeMethods.SHUTDOWN_PRIORITY_FIRST, 0);
        }

        public bool CreateLock(Control owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var machines = FindBlockingMachines();
            if (machines.Length < 1)
            {
                return false;
            }

            string machinesLabel = "";

            if (machines.Length == 1)
            {
                machinesLabel = machines[0].Name;
            }
            else if (machines.Length == 2)
            {
                machinesLabel = string.Join(" & ", machines.Select(m => m.Name));
            }
            else
            {
                machinesLabel = string.Join(", ", machines.Select(m => m.Name));
            }

            if (!NativeMethods.ShutdownBlockReasonCreate(owner.Handle, $"Shutting down {machinesLabel}"))
            {
                logger.LogError($"Failed to create shutdown block {new { LastError = Marshal.GetLastWin32Error() }}");
                return false;
            }

            logger.LogTrace("Created shutdown block");
            return true;
        }

        internal void RemoveLock(Control owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (!NativeMethods.ShutdownBlockReasonDestroy(owner.Handle))
            {
                logger.LogError($"Failed to destroy shutdown block {new { LastError = Marshal.GetLastWin32Error() }}");
                return;
            }

            logger.LogTrace("Destroyed shutdown block");
        }

        private IMachineMetadata[] FindBlockingMachines()
        {
            appState.UpdateMachines();
            return appState.GetMachines((m, c) => c?.AutoStop == true && !m.IsPoweredOff).ToArray();
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern bool SetProcessShutdownParameters(uint dwLevel, uint dwFlags);

            // https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-setprocessshutdownparameters
            public const int SHUTDOWN_PRIORITY_FIRST = 0x300;

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool ShutdownBlockReasonCreate(IntPtr hWnd,
                [MarshalAs(UnmanagedType.LPWStr)] string reason);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool ShutdownBlockReasonQuery(IntPtr hWnd,
                [MarshalAs(UnmanagedType.LPWStr)] StringBuilder reason, ref uint size);
        }
    }
}