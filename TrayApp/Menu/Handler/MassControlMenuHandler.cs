using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrayApp.State;
using TrayApp.VirtualMachine;

namespace TrayApp.Menu.Handler
{
    public class MassControlMenuHandler : IMenuHandler, IMenuHandlerUpdateAware, IDisposable
    {
        private readonly Dictionary<string, ToolStripItem> menuItems = new Dictionary<string, ToolStripItem>();
        private readonly MassController autoController;
        private readonly AppState appState;

        public MassControlMenuHandler(MassController autoController, AppState appState)
        {
            this.autoController = autoController ?? throw new ArgumentNullException(nameof(autoController));
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
        }

        public int GetSortOrder()
        {
            return 8;
        }

        public ToolStripItem[] CreateMenuItems()
        {
            DisposeMenuItems();

            if (appState.GetMachines().Count < 1)
            {
                return Array.Empty<ToolStripMenuItem>();
            }

            menuItems["separator"] = new ToolStripSeparator();

            menuItems["start"] = new ToolStripMenuItem(
                "&Start All",
                null,
                (object _, EventArgs __) => new Task(() => autoController.StartAll()).Start()
            );

            menuItems["stop"] = new ToolStripMenuItem(
                "S&top All",
                null,
                (object _, EventArgs __) => new Task(() => autoController.StopAll()).Start()
            );

            UpdateMenuItems();

            return menuItems.Values.ToArray();
        }

        public void UpdateMenuItems()
        {
            menuItems["start"].Enabled = appState.GetMachines().Any(machine => machine.IsPoweredOff) == true;
            menuItems["stop"].Enabled = appState.GetMachines().Any(machine => machine.IsPoweredOn) == true;
        }

        private void DisposeMenuItems()
        {
            if (menuItems != null)
            {
                foreach (var menuItem in menuItems.Values)
                {
                    menuItem.Dispose();
                }
                menuItems.Clear();
            }
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
                DisposeMenuItems();
            }
        }
    }
}