using System;
using System.Windows.Forms;
using TrayApp.KeepAwake;
using TrayApp.State;

namespace TrayApp.Menu.Handler
{
    public class KeepAwakeMenuHandler : IMenuHandler, IDisposable
    {
        private readonly KeepAwakeTask keepAwakeTask;
        private readonly AppState appState;
        private ToolStripMenuItem menuItem;

        public KeepAwakeMenuHandler(KeepAwakeTask keepAwakeTask, AppState appState)
        {
            this.keepAwakeTask = keepAwakeTask ?? throw new ArgumentNullException(nameof(keepAwakeTask));
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));

            appState.OnConfigurationChange += OnConfigurationChange;
        }

        private void OnConfigurationChange(object sender, ConfigurationChangeEventArgs e)
        {
            // TODO Should this instead be handled by TrayApplicationContext recreating the menu OnConfigurationChange?
            menuItem.Visible = e.NewConfiguration.ShowKeepAwakeMenu;
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
                Visible = appState.Configuration.ShowKeepAwakeMenu,
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