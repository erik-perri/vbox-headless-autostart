using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrayApp.VirtualMachine
{
    public class MachineStore
    {
        private readonly object machineLock = new object();
        private readonly ILogger<MachineStore> logger;
        private readonly List<IMachine> machines = new List<IMachine>();

        public event EventHandler OnMachineChange;

        public MachineStore(ILogger<MachineStore> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IMachine[] GetMachines()
        {
            lock (machineLock)
            {
                return machines.ToArray();
            }
        }

        internal void SetMachines(IMachine[] newMachines)
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

                    OnMachineChange?.Invoke(this, EventArgs.Empty);
                }
            }
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