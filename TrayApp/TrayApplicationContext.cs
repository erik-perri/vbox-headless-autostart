using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using TrayApp.Menu;
using TrayApp.VirtualMachine;

namespace TrayApp
{
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly object updateLock = new object();
        private readonly ILogger<TrayApplicationContext> logger;
        private readonly TrayContextMenuStrip contextMenu;

        public TrayApplicationContext(
            ILogger<TrayApplicationContext> logger,
            TrayContextMenuStrip contextMenu,
            NotifyIconManager notifyIconManager,
            MachineStore machineStore
        )
        {
            logger.LogTrace(".ctor");

            if (notifyIconManager == null)
            {
                throw new ArgumentNullException(nameof(notifyIconManager));
            }

            if (machineStore == null)
            {
                throw new ArgumentNullException(nameof(machineStore));
            }

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextMenu = contextMenu ?? throw new ArgumentNullException(nameof(contextMenu));

            notifyIconManager.NotifyIcon.ContextMenuStrip = contextMenu;
            notifyIconManager.ShowIcon();

            machineStore.OnMachineChange += (object _, EventArgs __) => CreateContextMenu();
            machineStore.OnStateChange += (object _, EventArgs __) => UpdateContextMenu();

            CreateContextMenu();
        }

        private void CreateContextMenu()
        {
            lock (updateLock)
            {
                if (contextMenu.InvokeRequired)
                {
                    logger.LogTrace("CreateContextMenu (Invoking)");

                    contextMenu.Invoke(new MethodInvoker(delegate { contextMenu.CreateContextMenu(); }));
                    return;
                }

                logger.LogTrace("CreateContextMenu");

                contextMenu.CreateContextMenu();
            }
        }

        private void UpdateContextMenu()
        {
            lock (updateLock)
            {
                if (contextMenu.InvokeRequired)
                {
                    logger.LogTrace("UpdateContextMenu (Invoking)");

                    contextMenu.Invoke(new MethodInvoker(delegate { contextMenu.UpdateContextMenu(); }));
                    return;
                }

                logger.LogTrace("UpdateContextMenu");

                contextMenu.UpdateContextMenu();
            }
        }

        protected override void Dispose(bool disposing)
        {
            logger.LogTrace($"Dispose({disposing})");

            base.Dispose(disposing);
        }
    }
}