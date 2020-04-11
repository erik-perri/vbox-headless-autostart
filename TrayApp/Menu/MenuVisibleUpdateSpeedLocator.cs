using System;
using TrayApp.VirtualMachine;

namespace TrayApp.Menu
{
    public class MenuVisibleUpdateSpeedLocator : IUpdateSpeedLocator
    {
        private readonly NotifyIconManager notifyIconManager;

        public MenuVisibleUpdateSpeedLocator(NotifyIconManager notifyIconManager)
        {
            this.notifyIconManager = notifyIconManager ?? throw new ArgumentNullException(nameof(notifyIconManager));
        }

        public int GetUpdateSpeed()
        {
            return notifyIconManager.NotifyIcon.ContextMenuStrip.Visible ? 1000 : 10000;
        }
    }
}