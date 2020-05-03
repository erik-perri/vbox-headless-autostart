using System;
using System.Windows.Forms;
using TrayApp.Configuration;

namespace TrayApp.Menu.Handler
{
    public class ConfigureMenuHandler : IMenuHandler, IDisposable
    {
        private readonly ConfigurationUpdater configurationUpdater;
        private ToolStripMenuItem menuItem;

        public ConfigureMenuHandler(ConfigurationUpdater configurationUpdater, NotifyIconManager notifyIconManager)
        {
            this.configurationUpdater = configurationUpdater ?? throw new ArgumentNullException(nameof(configurationUpdater));

            if (notifyIconManager is null)
            {
                throw new ArgumentNullException(nameof(notifyIconManager));
            }

            notifyIconManager.NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        public int GetSortOrder()
        {
            return 0;
        }

        public ToolStripItem[] CreateMenuItems()
        {
            menuItem?.Dispose();
            menuItem = new ToolStripMenuItem("&Configure", Properties.Resources.Settings, OnConfigure);

            return new ToolStripItem[] { menuItem };
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            configurationUpdater.ShowConfigurationForm();
        }

        private void OnConfigure(object sender, EventArgs eventArgs)
        {
            configurationUpdater.ShowConfigurationForm();
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
                menuItem?.Dispose();
                menuItem = null;
            }
        }
    }
}