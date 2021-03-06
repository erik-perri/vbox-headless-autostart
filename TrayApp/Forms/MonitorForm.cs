﻿using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using TrayApp.Configuration;
using TrayApp.Shutdown;
using TrayApp.VirtualMachine;

namespace TrayApp.Forms
{
    public partial class MonitorForm : Form
    {
        private readonly ILogger<MonitorForm> logger;
        private readonly ShutdownLocker shutdownMonitor;
        private readonly MassController autoController;
        private readonly ConfigurationUpdater configurationUpdater;
        private bool shutdownLocked;

        private static readonly uint ConfigureMessage = NativeMethods.RegisterWindowMessage(
            $"{Assembly.GetEntryAssembly()?.GetName().Name}\\Configure"
        );

        public MonitorForm(
            ILogger<MonitorForm> logger,
            ShutdownLocker shutdownMonitor,
            MassController autoController,
            ConfigurationUpdater configurationUpdater
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.shutdownMonitor = shutdownMonitor ?? throw new ArgumentNullException(nameof(shutdownMonitor));
            this.autoController = autoController ?? throw new ArgumentNullException(nameof(autoController));
            this.configurationUpdater = configurationUpdater ?? throw new ArgumentNullException(nameof(configurationUpdater));

            InitializeComponent();
        }

        public static void BroadcastConfigureMessage()
        {
            NativeMethods.SendNotifyMessage(NativeMethods.HWND_BROADCAST, ConfigureMessage, UIntPtr.Zero, IntPtr.Zero);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == ConfigureMessage)
            {
                configurationUpdater.ShowConfigurationForm();
                return;
            }

            switch (m.Msg)
            {
                // When Windows asks if we're ready to end the session create a block and tell it no
                case NativeMethods.WM_QUERYENDSESSION:
                    logger.LogTrace($"WndProc received {new { Msg = "WM_QUERYENDSESSION", m.WParam, m.LParam }}");

                    if (shutdownMonitor.CreateLock(this))
                    {
                        shutdownLocked = true;
                        m.Result = new IntPtr(1);
                        return;
                    }
                    break;

                // When Windows is actually ending the session stop all machines, remove the lock, and exit
                case NativeMethods.WM_ENDSESSION:
                    logger.LogTrace($"WndProc received {new { Msg = "WM_ENDSESSION", m.WParam, m.LParam }}");

                    if (!shutdownLocked)
                    {
                        break;
                    }

                    logger.LogInformation("Stopping machines due to system shutdown");
                    autoController.StopAll(
                        (_, c) => c?.AutoStop == true,
                        () =>
                        {
                            // Update the shutdown lock message
                            shutdownMonitor.CreateLock(this);
                        }
                    );

                    // If we don't wait here we end up with VirtualBox hanging the shutdown complaining about open
                    // connections
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
            logger.LogDebug("Machine stop complete, waiting for VirtualBox");

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < 10000)
            {
                var processes = Process.GetProcesses().Count(FilterVirtualBoxProcesses);
                if (processes < 1)
                {
                    break;
                }

                logger.LogDebug($"Still waiting for VirtualBox, processes left: {processes}");
                Thread.Sleep(1000);
            }

            // VBoxSVC should close when it is no longer needed, if it hasn't we will attempt to kill it to prevent
            // Windows from blocking shutdown due to "VirtualBox Interface has active connections".  If we are unable
            // to kill it the process is likely owned by someone else.
            var processesLeft = Process.GetProcesses().Where(FilterVirtualBoxProcesses).ToList();
            if (processesLeft.Count == 1 && processesLeft[0].ProcessName == "VBoxSVC")
            {
                try
                {
                    var process = processesLeft[0];

                    // In my testing it takes 3-4 seconds for it to close after the last machine is closed, this plus
                    // the 1000ms wait above will hopefully be plenty of time if the process is going to close itself.
                    Thread.Sleep(5000);

                    if (process.HasExited)
                    {
                        return;
                    }

                    process.Kill();
                    if (process.WaitForExit(1000))
                    {
                        logger.LogInformation($"Killed rogue VBoxSVC process, {process.Id}");
                    }
                    else
                    {
                        logger.LogInformation($"Failed to kill rogue VBoxSVC process, WaitForExit failed");
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    logger.LogInformation($"Failed to kill VBoxSVC process, {e.Message}");
                }
            }
        }

        private bool FilterVirtualBoxProcesses(Process process)
        {
            return process.ProcessName.StartsWith("VBox", StringComparison.OrdinalIgnoreCase) &&
                !process.ProcessName.Equals("VBoxHeadlessAutoStart", StringComparison.OrdinalIgnoreCase) &&
                !process.ProcessName.Equals("VBoxSDS", StringComparison.OrdinalIgnoreCase);
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

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern uint RegisterWindowMessage(string lpString);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

            public static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        }
    }
}