using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrayApp.VirtualMachine;

namespace TrayApp.Menu.Handler
{
    public class MachineControlMenuHandler : IMenuHandler, IMenuHandlerUpdateAware, IDisposable
    {
        private readonly Dictionary<string, ToolStripItem> menuItems = new Dictionary<string, ToolStripItem>();
        private readonly ILogger<MachineControlMenuHandler> logger;
        private readonly IMachineController machineController;
        private readonly MachineStore machineStore;

        public MachineControlMenuHandler(
            ILogger<MachineControlMenuHandler> logger,
            IMachineController machineController,
            MachineStore machineStore
        )
        {
            logger.LogTrace(".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.machineController = machineController ?? throw new ArgumentNullException(nameof(machineController));
            this.machineStore = machineStore ?? throw new ArgumentNullException(nameof(machineStore));
        }

        public int GetSortOrder()
        {
            return 4;
        }

        public ToolStripItem[] CreateMenuItems()
        {
            DisposeMenuItems();

            menuItems["separator"] = new ToolStripSeparator();

            var machines = machineStore.GetMachines();
            if (machines == null)
            {
                return Array.Empty<ToolStripItem>();
            }

            foreach (var machine in machines)
            {
                menuItems[machine.Uuid] = new ToolStripMenuItem(
                    machine.Name,
                    GetMenuImage(machine.Metadata.State),
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
            var machines = machineStore.GetMachines();
            if (machines == null)
            {
                return;
            }

            foreach (var machine in machines)
            {
                if (!menuItems.ContainsKey(machine.Uuid))
                {
                    continue;
                }

                menuItems[machine.Uuid].Image = GetMenuImage(machine.Metadata.State);
                if (menuItems[machine.Uuid] is ToolStripMenuItem toolStripMenuItem)
                {
                    foreach (ToolStripItem item in toolStripMenuItem.DropDownItems)
                    {
                        item.Enabled = false;

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

        private ToolStripItem[] BuildSubMenu(IMachine machine)
        {
            return new ToolStripItem[]
            {
                new ToolStripMenuItem(
                    "Headless start",
                    null,
                    (object _, EventArgs __) => new Task(() => machineController.StartMachineHeadless(machine)).Start()
                ),
                new ToolStripMenuItem(
                    "Normal start",
                    null,
                    (object _, EventArgs __) => new Task(() => machineController.StartMachine(machine)).Start()
                ),
                new ToolStripSeparator(),
                new ToolStripMenuItem(
                    "Save State",
                    null,
                    (object _, EventArgs __) => new Task(() => machineController.SaveState(machine)).Start()
                ),
                new ToolStripMenuItem(
                    "ACPI Shutdown",
                    null,
                    (object _, EventArgs __) => new Task(() => machineController.AcpiPowerOff(machine, 90000, () => { })).Start()
                ),
                new ToolStripMenuItem(
                    "Power off",
                    null,
                    (object _, EventArgs __) => new Task(() => machineController.PowerOff(machine)).Start()
                ),
                new ToolStripSeparator(),
                new ToolStripMenuItem(
                    "Reset",
                    null,
                    (object _, EventArgs __) => machineController.Reset(machine)
                )
            };
        }

        private System.Drawing.Bitmap GetMenuImage(MachineState state)
        {
            switch (state)
            {
                case MachineState.Aborted:
                    return Properties.Resources.VirtualMachineError;

                case MachineState.PoweredOff:
                    return Properties.Resources.VirtualMachineStop;

                case MachineState.StateSaved:
                    return Properties.Resources.VirtualMachinePause;

                case MachineState.Running:
                    return Properties.Resources.VirtualMachineRunning;

                case MachineState.Saving:
                case MachineState.Restoring:
                    return Properties.Resources.VirtualMachineRefresh;
            }

            return Properties.Resources.VirtualMachine;
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
            logger.LogTrace($"Dispose({disposing})");

            if (disposing)
            {
                DisposeMenuItems();
            }
        }
    }
}