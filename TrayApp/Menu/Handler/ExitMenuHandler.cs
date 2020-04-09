using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace TrayApp.Menu.Handler
{
    public class ExitMenuHandler : IMenuHandler, IDisposable
    {
        private readonly ILogger<ExitMenuHandler> logger;
        private readonly TrayIconManager trayIconManager;
        private ToolStripItem menuItemSeparator;
        private ToolStripItem menuItemExit;

        public ExitMenuHandler(ILogger<ExitMenuHandler> logger, TrayIconManager trayIconManager)
        {
            logger.LogTrace(".ctor");

            this.logger = logger;
            this.trayIconManager = trayIconManager;
        }

        public int GetSortOrder()
        {
            return 10;
        }

        public ToolStripItem[] CreateMenuItems()
        {
            menuItemSeparator?.Dispose();
            menuItemSeparator = new ToolStripSeparator();

            menuItemExit?.Dispose();
            menuItemExit = new ToolStripMenuItem("E&xit", null, OnExit);

            return new ToolStripItem[] {
                menuItemSeparator,
                menuItemExit,
            };
        }

        private void OnExit(object sender, EventArgs e)
        {
            // If the icon is not hidden before exiting it will remain in the tray until the user hovers over it
            trayIconManager.Hide();

            Application.Exit();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            logger.LogTrace($"Dispose({disposing})");

            if (disposing)
            {
                menuItemSeparator?.Dispose();
                menuItemExit?.Dispose();
            }
        }
    }
}