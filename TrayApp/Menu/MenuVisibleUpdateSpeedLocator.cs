using System;

namespace TrayApp.Menu
{
    public class MenuVisibleUpdateSpeedLocator
    {
        private readonly NotifyIconManager notifyIconManager;

        public MenuVisibleUpdateSpeedLocator(NotifyIconManager notifyIconManager)
        {
            this.notifyIconManager = notifyIconManager ?? throw new ArgumentNullException(nameof(notifyIconManager));
        }

        public int GetUpdateSpeed()
        {
            return notifyIconManager.NotifyIcon.ContextMenuStrip?.Visible == true ? 100 : 5000;
        }
    }
}