using CommonLib.VirtualMachine;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TrayApp.Configuration;
using TrayApp.VirtualMachine;

namespace TrayApp.AutoControl
{
    public class ShutdownMonitor
    {
        private readonly ILogger<ShutdownMonitor> logger;
        private readonly MachineStore machineStore;
        private readonly ConfigurationStore configurationStore;
        private readonly ShutdownBlock shutdownBlock;

        public bool Blocking { get; private set; }

        public ShutdownMonitor(
            ILogger<ShutdownMonitor> logger,
            MachineStore machineStore,
            ConfigurationStore configurationStore,
            ShutdownBlock shutdownBlock
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.machineStore = machineStore ?? throw new ArgumentNullException(nameof(machineStore));
            this.configurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));
            this.shutdownBlock = shutdownBlock ?? throw new ArgumentNullException(nameof(shutdownBlock));

            machineStore.OnStateChange += UpdateLock;
        }

        public void UpdateLock()
        {
            UpdateLock(null, null);
        }

        private void UpdateLock(object _, EventArgs __)
        {
            var blockingMachines = FindBlockingMachines();

            logger.LogDebug($"Blocking machines {blockingMachines.Length}");

            if (blockingMachines.Length < 1)
            {
                shutdownBlock.StopBlocking();
                Blocking = false;
            }
            else
            {
                var reason = BuildReason(blockingMachines);
                if (shutdownBlock.StartBlocking(reason))
                {
                    Blocking = true;
                }
            }
        }

        private string BuildReason(IMachineMetadata[] blockingMachines)
        {
            if (blockingMachines == null)
            {
                throw new ArgumentNullException(nameof(blockingMachines));
            }

            if (blockingMachines.Length < 1)
            {
                throw new ArgumentException("No blocking machines specified", nameof(blockingMachines));
            }

            var machineText = blockingMachines.Length == 1 ? "machine" : "machines";

            return $"{blockingMachines.Length} {machineText} left to power off.";
        }

        private IMachineMetadata[] FindBlockingMachines()
        {
            return machineStore.GetMachines()
                .Where(v => !v.IsPoweredOff && configurationStore.GetConfiguration().Machines.Any(c => c.Uuid == v.Uuid))
                .ToArray();
        }
    }
}