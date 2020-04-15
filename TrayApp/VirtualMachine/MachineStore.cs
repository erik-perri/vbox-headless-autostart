using CommonLib.VirtualMachine;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TrayApp.Helpers;

namespace TrayApp.VirtualMachine
{
    public class MachineStore
    {
        private readonly List<IMachineMetadata> machines = new List<IMachineMetadata>();

        private readonly ILogger<MachineStore> logger;
        private readonly IMachineLocator machineLocator;
        private readonly MonitoredMachineFilter machineFilter;

        public event EventHandler OnMachineChange;

        public event EventHandler OnStateChange;

        public MachineStore(
            ILogger<MachineStore> logger,
            IMachineLocator machineLocator,
            MonitoredMachineFilter machineFilter
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.machineLocator = machineLocator ?? throw new ArgumentNullException(nameof(machineLocator));
            this.machineFilter = machineFilter ?? throw new ArgumentNullException(nameof(machineFilter));
        }

        public IMachineMetadata[] GetMachines()
        {
            return machines.ToArray();
        }

        public void UpdateMachines()
        {
            SetMachines(machineLocator.ListMachines(machineFilter));
        }

        private void SetMachines(IMachineMetadata[] newMachines)
        {
            if (newMachines == null)
            {
                throw new ArgumentNullException(nameof(newMachines));
            }

            var oldMachines = machines.ToArray();
            machines.Clear();
            machines.AddRange(newMachines);

            if (!newMachines.OrderBy(m => m.Uuid).SequenceEqual(oldMachines.OrderBy(m => m.Uuid)))
            {
                DumpMachineListChanges(oldMachines, newMachines);

                if (WasMachineListChanged(oldMachines, newMachines))
                {
                    OnMachineChange?.Invoke(this, EventArgs.Empty);
                }

                OnStateChange?.Invoke(this, EventArgs.Empty);
            }
        }

        private static bool WasMachineListChanged(IMachineMetadata[] oldMachines, IMachineMetadata[] newMachines)
        {
            if (oldMachines == null)
            {
                throw new ArgumentNullException(nameof(oldMachines));
            }

            if (newMachines == null)
            {
                throw new ArgumentNullException(nameof(newMachines));
            }

            return oldMachines == null
                || newMachines.Except(oldMachines, new UuidEqualityComparer()).Any()
                || oldMachines.Except(newMachines, new UuidEqualityComparer()).Any();
        }

        private void DumpMachineListChanges(IMachineMetadata[] oldMachines, IMachineMetadata[] newMachines)
        {
            if (oldMachines == null)
            {
                throw new ArgumentNullException(nameof(oldMachines));
            }

            if (newMachines == null)
            {
                throw new ArgumentNullException(nameof(newMachines));
            }

            logger.LogDebug("Machine store changed");

            var added = newMachines.Except(oldMachines, new UuidEqualityComparer());
            var removed = oldMachines.Except(newMachines, new UuidEqualityComparer());

            foreach (var machine in added)
            {
                logger.LogDebug($" - Added {machine}");
            }

            foreach (var machine in removed)
            {
                logger.LogDebug($" - Removed {machine}");
            }

            foreach (var newMachine in newMachines)
            {
                var oldMachine = Array.Find(oldMachines, m => m.Uuid == newMachine.Uuid);
                if (oldMachine?.Equals(newMachine) == false)
                {
                    logger.LogDebug($" - Changed");
                    logger.LogDebug($"     Old {oldMachine}");
                    logger.LogDebug($"     New {newMachine}");
                }
            }
        }
    }
}