using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using TrayApp.Menu.Handler;
using TrayApp.State;

namespace TrayApp.Menu
{
    public class TrayContextMenuStrip : ContextMenuStrip
    {
        private readonly ReadOnlyCollection<IMenuHandler> handlers;

        public TrayContextMenuStrip(TrayHandlerCollection handlerCollection, MachineStateUpdater stateUpdater)
        {
            handlers = handlerCollection?.Handlers ?? throw new ArgumentNullException(nameof(handlerCollection));

            Opening += (object sender, System.ComponentModel.CancelEventArgs e) =>
                stateUpdater.RequestFastUpdates("ContextMenuOpen");

            Closing += (object sender, ToolStripDropDownClosingEventArgs e) =>
                stateUpdater.RemoveFastUpdateRequest("ContextMenuOpen");
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