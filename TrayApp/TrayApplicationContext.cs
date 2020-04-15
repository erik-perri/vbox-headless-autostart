using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using TrayApp.Menu;
using TrayApp.State;

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
            AppState appState
        )
        {
            if (notifyIconManager == null)
            {
                throw new ArgumentNullException(nameof(notifyIconManager));
            }

            if (appState == null)
            {
                throw new ArgumentNullException(nameof(appState));
            }

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextMenu = contextMenu ?? throw new ArgumentNullException(nameof(contextMenu));

            notifyIconManager.NotifyIcon.ContextMenuStrip = contextMenu;
            notifyIconManager.ShowIcon();

            appState.OnMachineListChange += (object _, EventArgs __) => CreateContextMenu();
            appState.OnMachineStateChange += (object _, EventArgs __) => UpdateContextMenu();

            CreateContextMenu();
        }

        private void CreateContextMenu()
        {
            lock (updateLock)
            {
                logger.LogTrace($"Creating context menu {new { contextMenu.InvokeRequired }}");

                if (contextMenu.InvokeRequired)
                {
                    contextMenu.Invoke(new MethodInvoker(delegate { contextMenu.CreateContextMenu(); }));
                    return;
                }

                contextMenu.CreateContextMenu();
            }
        }

        private void UpdateContextMenu()
        {
            lock (updateLock)
            {
                logger.LogTrace($"Updating context menu {new { contextMenu.InvokeRequired }}");

                if (contextMenu.InvokeRequired)
                {
                    contextMenu.Invoke(new MethodInvoker(delegate { contextMenu.UpdateContextMenu(); }));
                    return;
                }

                contextMenu.UpdateContextMenu();
            }
        }
    }
}