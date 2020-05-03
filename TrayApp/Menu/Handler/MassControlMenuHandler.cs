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

            if (!appState.HasMachines((_, c) => c?.ShowMenu == true))
            {
                return Array.Empty<ToolStripMenuItem>();
            }

            menuItems["separator"] = new ToolStripSeparator();

            menuItems["start"] = new ToolStripMenuItem(
                "&Start All",
                null,
                (_, __) => new Task(() => autoController.StartAll((___, c) => c?.ShowMenu == true)).Start()
            );

            menuItems["stop"] = new ToolStripMenuItem(
                "S&top All",
                null,
                (_, __) => new Task(() => autoController.StopAll((___, c) => c?.ShowMenu == true)).Start()
            );

            UpdateMenuItems();

            return menuItems.Values.ToArray();
        }

        public void UpdateMenuItems()
        {
            if (menuItems.Count < 1)
            {
                return;
            }

            menuItems["start"].Enabled = appState.HasMachines((m, c) => c?.ShowMenu == true && m.IsPoweredOff);
            menuItems["stop"].Enabled = appState.HasMachines((m, c) => c?.ShowMenu == true && m.IsPoweredOn);
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