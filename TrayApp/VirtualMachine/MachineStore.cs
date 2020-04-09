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
                var previousMachines = machines.ToArray();
                machines.Clear();
                machines.AddRange(newMachines);

                if (!newMachines.OrderBy(m => m.Uuid).SequenceEqual(previousMachines.OrderBy(m => m.Uuid)))
                {
                    DumpChanges(previousMachines, newMachines);

                    OnMachineChange?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void DumpChanges(IMachine[] oldMachines, IMachine[] newMachines)
        {
            logger.LogDebug("Machines modified");

            foreach (var newMachine in newMachines)
            {
                if (oldMachines == null)
                {
                    logger.LogDebug($" - Machine added: \"{newMachine.Uuid}\"");
                    continue;
                }

                var oldMachine = Array.Find(oldMachines, m => m.Uuid == newMachine.Uuid);
                if (oldMachine == null)
                {
                    logger.LogDebug($" - Machine added: \"{newMachine.Uuid}\"");
                    continue;
                }

                if (!oldMachine.Equals(newMachine))
                {
                    logger.LogDebug($" - Machine changed: \"{newMachine.Uuid}\"");
                    logger.LogDebug("     Old: {machine}", new
                    {
                        oldMachine.Uuid,
                        oldMachine.Name,
                        oldMachine.Metadata,
                    });
                    logger.LogDebug("     New: {machine}", new
                    {
                        newMachine.Uuid,
                        newMachine.Name,
                        newMachine.Metadata,
                    });
                }
            }

            if (oldMachines != null)
            {
                foreach (var oldMachine in oldMachines)
                {
                    if (Array.Find(newMachines, m => m.Uuid == oldMachine.Uuid) == null)
                    {
                        logger.LogDebug($" - Machine removed: \"{oldMachine.Uuid}\"");
                    }
                }
            }
        }
    }
}