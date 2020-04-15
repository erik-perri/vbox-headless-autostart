using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Windows.Forms;
using TrayApp.AutoControl;

namespace TrayApp.Forms
{
    public partial class ShutdownMonitorForm : Form
    {
        private readonly ILogger<ShutdownMonitorForm> logger;
        private readonly ShutdownMonitor shutdownMonitor;
        private readonly AutoController autoController;

        public ShutdownMonitorForm(
            ILogger<ShutdownMonitorForm> logger,
            ShutdownMonitor shutdownMonitor,
            AutoController autoController
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.shutdownMonitor = shutdownMonitor ?? throw new ArgumentNullException(nameof(shutdownMonitor));
            this.autoController = autoController ?? throw new ArgumentNullException(nameof(autoController));

            InitializeComponent();

            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
        }

        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            logger.LogDebug($"SystemEvents_SessionEnding {new { e.Reason }}");

            if (shutdownMonitor.Blocking)
            {
                e.Cancel = true;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)NativeMethods.WM.QUERYENDSESSION:
                case (int)NativeMethods.WM.ENDSESSION:
                    var Msg = $"{m.Msg}";
                    if (Enum.TryParse($"{Msg}", out NativeMethods.WM messageName))
                    {
                        Msg = messageName.ToString();
                    }

                    logger.LogDebug($"WndProc received {new { Msg, m.WParam, m.LParam }}");

                    shutdownMonitor.UpdateLock();
                    if (shutdownMonitor.Blocking)
                    {
                        autoController.StopMachines();

                        // Don't run base.WndProc to let Windows know we want to block shutdown
                        return;
                    }
                    break;
            }

            base.WndProc(ref m);
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
            public enum WM
            {
                /// <summary>
                /// The WM_QUERYENDSESSION message is sent when the user chooses to end the session or when an application calls one of the system shutdown functions. If any application returns zero, the session is not ended. The system stops sending WM_QUERYENDSESSION messages as soon as one application returns zero.
                /// After processing this message, the system sends the WM_ENDSESSION message with the wParam parameter set to the results of the WM_QUERYENDSESSION message.
                /// </summary>
                QUERYENDSESSION = 0x0011,

                /// <summary>
                /// The WM_ENDSESSION message is sent to an application after the system processes the results of the WM_QUERYENDSESSION message. The WM_ENDSESSION message informs the application whether the session is ending.
                /// </summary>
                ENDSESSION = 0x0016,
            }
        }
    }
}