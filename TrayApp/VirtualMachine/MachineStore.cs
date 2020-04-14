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
        private readonly List<IMachine> machines = new List<IMachine>();

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

        public IMachine[] GetMachines()
        {
            return machines.ToArray();
        }

        public void UpdateMachines()
        {
            logger.LogTrace("Updating machine states");

            SetMachines(machineLocator.ListMachinesWithMetadata(machineFilter));
        }

        private void SetMachines(IMachine[] newMachines)
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

        private static bool WasMachineListChanged(IMachine[] oldMachines, IMachine[] newMachines)
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

        private void DumpMachineListChanges(IMachine[] oldMachines, IMachine[] newMachines)
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
                logger.LogDebug($" - Added {new { machine.Uuid, machine.Name, machine.Metadata }}");
            }

            foreach (var machine in removed)
            {
                logger.LogDebug($" - Removed {new { machine.Uuid, machine.Name, machine.Metadata }}");
            }

            foreach (var newMachine in newMachines)
            {
                var oldMachine = Array.Find(oldMachines, m => m.Uuid == newMachine.Uuid);
                if (oldMachine?.Equals(newMachine) == false)
                {
                    logger.LogDebug($" - Changed");
                    logger.LogDebug($"     Old {new { oldMachine.Uuid, oldMachine.Name, oldMachine.Metadata }}");
                    logger.LogDebug($"     New {new { newMachine.Uuid, newMachine.Name, newMachine.Metadata }}");
                }
            }
        }
    }
}