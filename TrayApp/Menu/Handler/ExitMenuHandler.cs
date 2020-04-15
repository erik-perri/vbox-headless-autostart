using System;
using System.Windows.Forms;

namespace TrayApp.Menu.Handler
{
    public class ExitMenuHandler : IMenuHandler, IDisposable
    {
        private readonly NotifyIconManager notifyIconManager;
        private ToolStripItem menuItemSeparator;
        private ToolStripItem menuItemExit;

        public ExitMenuHandler(NotifyIconManager notifyIconManager)
        {
            this.notifyIconManager = notifyIconManager ?? throw new ArgumentNullException(nameof(notifyIconManager));
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
            notifyIconManager.HideIcon();

            Application.Exit();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                menuItemSeparator?.Dispose();
                menuItemExit?.Dispose();
            }
        }
    }
}