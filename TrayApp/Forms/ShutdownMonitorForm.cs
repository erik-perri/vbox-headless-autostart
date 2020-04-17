using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TrayApp.Shutdown;
using TrayApp.VirtualMachine;

namespace TrayApp.Forms
{
    public partial class ShutdownMonitorForm : Form
    {
        private readonly ILogger<ShutdownMonitorForm> logger;
        private readonly ShutdownLocker shutdownMonitor;
        private readonly MassController autoController;

        public ShutdownMonitorForm(
            ILogger<ShutdownMonitorForm> logger,
            ShutdownLocker shutdownMonitor,
            MassController autoController
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.shutdownMonitor = shutdownMonitor ?? throw new ArgumentNullException(nameof(shutdownMonitor));
            this.autoController = autoController ?? throw new ArgumentNullException(nameof(autoController));

            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // When Windows asks if we're ready to end the session create a block and tell it no
                case NativeMethods.WM_QUERYENDSESSION:
                    logger.LogDebug($"WndProc received {new { Msg = "WM_QUERYENDSESSION", m.WParam, m.LParam }}");

                    if (shutdownMonitor.CreateLock(this))
                    {
                        m.Result = new IntPtr(1);
                        return;
                    }
                    break;

                // When Windows is actually ending the session stop all machines, remove the lock, and exit
                case NativeMethods.WM_ENDSESSION:
                    logger.LogDebug($"WndProc received {new { Msg = "WM_ENDSESSION", m.WParam, m.LParam }}");

                    logger.LogInformation("Stopping down machines due to system shutdown");
                    autoController.StopAll();

                    // If we don't wait here we end up with VirtualBox hanging the shutdown complaining about open
                    // connections
                    logger.LogInformation("Machine stop complete, waiting for VirtualBox");
                    WaitForVirtualBoxToFinish();

                    shutdownMonitor.RemoveLock(this);
                    Close();
                    Application.Exit();
                    break;
            }

            base.WndProc(ref m);
        }

        private void WaitForVirtualBoxToFinish()
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            List<Process> processes;

            while (stopwatch.ElapsedMilliseconds < 20000)
            {
                processes = Process.GetProcesses().Where(
                    p => p.ProcessName.StartsWith("VBox", StringComparison.OrdinalIgnoreCase)
                ).ToList();

                if (processes.Count < 1)
                {
                    break;
                }

                logger.LogDebug($"Processes beginning with VBox: {processes.Count}");
                Thread.Sleep(1000);
            }

            processes = Process.GetProcesses().Where(
                p => p.ProcessName.StartsWith("VBox", StringComparison.OrdinalIgnoreCase)
            ).ToList();
            if (processes.Count > 0)
            {
                foreach (var process in processes)
                {
                    logger.LogDebug($" - {process.ProcessName}.exe:{process.Id}");
                }
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            // Prevent the window showing with the initial Show call
            if (!IsHandleCreated)
            {
                CreateHandle();
                value = false;
            }

            base.SetVisibleCore(value);
        }

        private static class NativeMethods
        {
            /// <summary>
            /// <para>
            /// The WM_QUERYENDSESSION message is sent when the user chooses to end the session or when an
            /// application calls one of the system shutdown functions. If any application returns zero, the
            /// session is not ended. The system stops sending WM_QUERYENDSESSION messages as soon as one
            /// application returns zero.
            /// </para>
            /// <para>
            /// After processing this message, the system sends the WM_ENDSESSION message with the wParam parameter
            /// set to the results of the WM_QUERYENDSESSION message.
            /// </para>
            /// </summary>
            public const int WM_QUERYENDSESSION = 0x0011;

            /// <summary>
            /// The WM_ENDSESSION message is sent to an application after the system processes the results of the
            /// WM_QUERYENDSESSION message. The WM_ENDSESSION message informs the application whether the session
            /// is ending.
            /// </summary>
            public const int WM_ENDSESSION = 0x0016;
        }
    }
}