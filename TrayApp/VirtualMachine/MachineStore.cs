using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrayApp.VirtualMachine
{
    public class MachineStore
    {
        private readonly object machineLock = new object();
        private readonly List<IMachine> machines = new List<IMachine>();

        private readonly ILogger<MachineStore> logger;
        private readonly ILocatorService locatorService;
        private readonly MonitoredMachineFilter machineFilter;

        public event EventHandler OnMachineChange;

        public event EventHandler OnStateChange;

        public MachineStore(
            ILogger<MachineStore> logger,
            ILocatorService locatorService,
            MonitoredMachineFilter machineFilter
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.locatorService = locatorService ?? throw new ArgumentNullException(nameof(locatorService));
            this.machineFilter = machineFilter ?? throw new ArgumentNullException(nameof(machineFilter));
        }

        public IMachine[] GetMachines()
        {
            lock (machineLock)
            {
                return machines.ToArray();
            }
        }

        public void UpdateMachines()
        {
            lock (machineLock)
            {
                SetMachines(locatorService.LocateMachines(machineFilter, true));
            }
        }

        private void SetMachines(IMachine[] newMachines)
        {
            if (newMachines == null)
            {
                throw new ArgumentNullException(nameof(newMachines));
            }

            lock (machineLock)
            {
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
                    else
                    {
                        OnStateChange?.Invoke(this, EventArgs.Empty);
                    }
                }
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

            logger.LogDebug("Machines modified");

            var added = newMachines.Except(oldMachines, new UuidEqualityComparer());
            var removed = oldMachines.Except(newMachines, new UuidEqualityComparer());

            foreach (var machine in added)
            {
                logger.LogDebug($" - Added:   {new { machine.Uuid, machine.Name, machine.Metadata }}");
            }

            foreach (var machine in removed)
            {
                logger.LogDebug($" - Removed: {new { machine.Uuid, machine.Name, machine.Metadata }}");
            }

            foreach (var newMachine in newMachines)
            {
                var oldMachine = Array.Find(oldMachines, m => m.Uuid == newMachine.Uuid);
                if (oldMachine?.Equals(newMachine) == false)
                {
                    logger.LogDebug($" - Changed:");
                    logger.LogDebug($"     Old: {new { oldMachine.Uuid, oldMachine.Name, oldMachine.Metadata }}");
                    logger.LogDebug($"     New: {new { newMachine.Uuid, newMachine.Name, newMachine.Metadata }}");
                }
            }
        }
    }
}