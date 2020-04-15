using System;
using System.Windows.Forms;
using TrayApp.Menu.Handler;
using TrayApp.VirtualMachine;

namespace TrayApp.Menu
{
    public class TrayContextMenuStrip : ContextMenuStrip
    {
        private readonly IMenuHandler[] handlers;

        public TrayContextMenuStrip(IMenuHandler[] handlers, MachineStoreUpdater machineStoreUpdater)
        {
            this.handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));

            if (machineStoreUpdater == null)
            {
                throw new ArgumentNullException(nameof(machineStoreUpdater));
            }

            Opening += (object sender, System.ComponentModel.CancelEventArgs e) => machineStoreUpdater.RequestUpdate();
        }

        public void CreateContextMenu()
        {
            Items.Clear();

            foreach (var handler in handlers)
            {
                var items = handler.CreateMenuItems();
                if (items.Length > 0)
                {
                    Items.AddRange(items);
                }
            }
        }

        public void UpdateContextMenu()
        {
            foreach (var handler in handlers)
            {
                if (handler is IMenuHandlerUpdateAware updateHandler)
                {
                    updateHandler.UpdateMenuItems();
                }
            }
        }
    }
}