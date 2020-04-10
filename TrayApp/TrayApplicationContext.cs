using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using TrayApp.Menu;
using TrayApp.VirtualMachine;

namespace TrayApp
{
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly ILogger<TrayApplicationContext> logger;
        private readonly TrayIconManager iconManager;
        private readonly TrayContextMenuStrip contextMenu;
        private readonly MachineStoreUpdater machineStoreUpdater;

        public TrayApplicationContext(
            ILogger<TrayApplicationContext> logger,
            TrayIconManager iconManager,
            TrayContextMenuStrip contextMenu,
            MachineStoreUpdater machineUpdater
        )
        {
            logger.LogTrace(".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextMenu = contextMenu ?? throw new ArgumentNullException(nameof(contextMenu));
            this.iconManager = iconManager ?? throw new ArgumentNullException(nameof(iconManager));
            this.machineStoreUpdater = machineUpdater ?? throw new ArgumentNullException(nameof(machineUpdater));

            iconManager.SetContextMenu(contextMenu);
            iconManager.Show();

            contextMenu.CreateContextMenu();

            machineStoreUpdater.StartMonitor();
        }

        protected override void Dispose(bool disposing)
        {
            logger.LogTrace($"Dispose({disposing})");

            base.Dispose(disposing);
        }
    }
}