using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
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
            TrayIconManager iconManager,
            MachineStore machineStore
        )
        {
            logger.LogTrace(".ctor");

            if (iconManager == null)
            {
                throw new ArgumentNullException(nameof(iconManager));
            }

            if (machineStore == null)
            {
                throw new ArgumentNullException(nameof(machineStore));
            }

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextMenu = contextMenu ?? throw new ArgumentNullException(nameof(contextMenu));

            iconManager.SetContextMenu(contextMenu);
            iconManager.Show();

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

                    // TODO Same as UpdateContextMenu
                    Task.Factory.StartNew(
                        () => contextMenu.Invoke(new MethodInvoker(delegate { contextMenu.CreateContextMenu(); })),
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskScheduler.Default
                    );
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

                    // TODO Figure out why this doesn't work without a new thread. If it is run in this thread the menu
                    //      fails to update and stops responding to the mouse with no errors.  If Invoke is removed we get
                    //      the normal cannot update control from a different thread error.
                    Task.Factory.StartNew(
                        () => contextMenu.Invoke(new MethodInvoker(delegate { contextMenu.UpdateContextMenu(); })),
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskScheduler.Default
                    );
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