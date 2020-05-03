using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrayApp.State;
using TrayApp.VirtualMachine;

namespace TrayApp.Menu.Handler
{
    public class MachineControlMenuHandler : IMenuHandler, IMenuHandlerUpdateAware, IDisposable
    {
        private readonly Dictionary<string, ToolStripItem> menuItems = new Dictionary<string, ToolStripItem>();
        private readonly IMachineController machineController;
        private readonly AppState appState;

        public MachineControlMenuHandler(IMachineController machineController, AppState appState)
        {
            this.machineController = machineController ?? throw new ArgumentNullException(nameof(machineController));
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
        }

        public int GetSortOrder()
        {
            return 4;
        }

        public ToolStripItem[] CreateMenuItems()
        {
            DisposeMenuItems();

            menuItems["separator"] = new ToolStripSeparator();

            var machines = appState.GetMachines((_, c) => c?.ShowMenu == true).ToList();
            if (machines.Count < 1)
            {
                return Array.Empty<ToolStripItem>();
            }

            foreach (var machine in machines)
            {
                menuItems[machine.Uuid] = new ToolStripMenuItem(
                    machine.Name,
                    GetMenuImage(machine.State),
                    BuildSubMenu(machine)
                );
            }

            // If only the separator was assigned we can destroy it and return an empty array
            if (menuItems.Count == 1)
            {
                DisposeMenuItems();

                return Array.Empty<ToolStripItem>();
            }

            UpdateMenuItems();

            return menuItems.Values.ToArray();
        }

        public void UpdateMenuItems()
        {
            foreach (var machine in appState.GetMachines((_, c) => c?.ShowMenu == true))
            {
                if (!menuItems.ContainsKey(machine.Uuid))
                {
                    continue;
                }

                menuItems[machine.Uuid].Image = GetMenuImage(machine.State);
                if (menuItems[machine.Uuid] is ToolStripMenuItem toolStripMenuItem)
                {
                    foreach (ToolStripItem item in toolStripMenuItem.DropDownItems)
                    {
                        item.Enabled = false;

                        if (item is MachineStatusToolStripItem statusItem)
                        {
                            statusItem.Update(machine);
                            continue;
                        }

                        switch (item.Text)
                        {
                            case "Headless start":
                            case "Normal start":
                                item.Enabled = machine.IsPoweredOff;
                                break;

                            case "Save State":
                            case "ACPI Shutdown":
                            case "Power off":
                            case "Pause":
                            case "Reset":
                                item.Enabled = machine.IsPoweredOn;
                                break;
                        }
                    }
                }
            }
        }

        private ToolStripItem[] BuildSubMenu(IMachineMetadata machine)
        {
            return new ToolStripItem[]
            {
                new MachineStatusToolStripItem(machine),
                new ToolStripSeparator(),
                new ToolStripMenuItem(
                    "Headless start",
                    null,
                    (_, __) => new Task(() => machineController.Start(machine, true)).Start()
                ),
                new ToolStripMenuItem(
                    "Normal start",
                    null,
                    (_, __) => new Task(() => machineController.Start(machine, false)).Start()
                ),
                new ToolStripSeparator(),
                new ToolStripMenuItem(
                    "Save State",
                    null,
                    (_, __) => new Task(() => machineController.SaveState(machine)).Start()
                ),
                new ToolStripMenuItem(
                    "ACPI Shutdown",
                    null,
                    (_, __) => new Task(() => machineController.AcpiPowerOff(machine, 90000)).Start()
                ),
                new ToolStripMenuItem(
                    "Power off",
                    null,
                    (_, __) => new Task(() => machineController.PowerOff(machine)).Start()
                ),
                new ToolStripSeparator(),
                new ToolStripMenuItem(
                    "Reset",
                    null,
                    (_, __) => machineController.Reset(machine)
                )
            };
        }

        private static Bitmap GetMenuImage(VirtualMachineState state)
        {
            return state switch
            {
                VirtualMachineState.Aborted => Properties.Resources.VirtualMachineError,
                VirtualMachineState.PoweredOff => Properties.Resources.VirtualMachineStop,
                VirtualMachineState.Saved => Properties.Resources.VirtualMachinePause,
                VirtualMachineState.Running => Properties.Resources.VirtualMachineRunning,
                VirtualMachineState.Saving => Properties.Resources.VirtualMachineRefresh,
                VirtualMachineState.Starting => Properties.Resources.VirtualMachineRefresh,
                VirtualMachineState.Restoring => Properties.Resources.VirtualMachineRefresh,
                _ => Properties.Resources.VirtualMachine
            };
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

        private class MachineStatusToolStripItem : ToolStripMenuItem
        {
            public MachineStatusToolStripItem(IMachineMetadata machine)
            {
                Update(machine);
            }

            public void Update(IMachineMetadata machine)
            {
                var text = new StringBuilder();

                text.Append(Regex.Replace(machine.State.ToString(), "([a-z])_?([A-Z])", "$1 $2"));

                if (machine is MachineMetadata metadata)
                {
                    text.Append(" since ");

                    text.Append(
                        metadata.LastAction.Date == DateTime.Today
                            ? metadata.LastAction.ToString("t", CultureInfo.CurrentCulture)
                            : metadata.LastAction.ToString("g", CultureInfo.CurrentCulture)
                    );

                    if (!string.IsNullOrWhiteSpace(metadata.SessionName))
                    {
                        text.Append(" (").Append(metadata.SessionName).Append(')');
                    }
                }

                Text = text.ToString();
            }
        }
    }
}