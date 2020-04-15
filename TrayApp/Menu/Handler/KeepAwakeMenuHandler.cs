using System;
using System.Windows.Forms;
using TrayApp.Configuration;
using TrayApp.KeepAwake;

namespace TrayApp.Menu.Handler
{
    public class KeepAwakeMenuHandler : IMenuHandler, IDisposable
    {
        private readonly KeepAwakeTask keepAwakeTask;
        private readonly ConfigurationStore configurationStore;
        private ToolStripMenuItem menuItem;

        public KeepAwakeMenuHandler(KeepAwakeTask keepAwakeTask, ConfigurationStore configurationStore)
        {
            this.keepAwakeTask = keepAwakeTask ?? throw new ArgumentNullException(nameof(keepAwakeTask));
            this.configurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));

            configurationStore.OnConfigurationChange += OnConfigurationChange;
        }

        private void OnConfigurationChange(object sender, EventArgs e)
        {
            // TODO Should this instead be handled by TrayApplicationContext recreating the menu OnConfigurationChange?
            menuItem.Visible = configurationStore.GetConfiguration().ShowKeepAwakeMenu;
        }

        public int GetSortOrder()
        {
            return 2;
        }

        public ToolStripItem[] CreateMenuItems()
        {
            menuItem?.Dispose();
            menuItem = new ToolStripMenuItem("Keep host &awake", null, OnKeepAwake)
            {
                Checked = keepAwakeTask.IsRunning,
                Visible = configurationStore.GetConfiguration().ShowKeepAwakeMenu,
            };

            return new ToolStripItem[] { menuItem };
        }

        private void OnKeepAwake(object sender, EventArgs e)
        {
            if (keepAwakeTask.IsRunning)
            {
                keepAwakeTask.Stop();
            }
            else
            {
                keepAwakeTask.Start();
            }

            menuItem.Checked = keepAwakeTask.IsRunning;
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
            }
        }
    }
}