using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TrayApp.Configuration;
using TrayApp.VirtualMachine;

namespace TrayApp.AutoStart
{
    public class MachineAutoStarter
    {
        private readonly ILogger<MachineAutoStarter> logger;
        private readonly MachineStore machineStore;
        private readonly ConfigurationStore configurationStore;
        private readonly IMachineController machineController;

        public MachineAutoStarter(
            ILogger<MachineAutoStarter> logger,
            MachineStore machineStore,
            ConfigurationStore configurationStore,
            IMachineController machineController
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.machineStore = machineStore ?? throw new ArgumentNullException(nameof(machineStore));
            this.configurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));
            this.machineController = machineController ?? throw new ArgumentNullException(nameof(machineController));
        }

        public bool StartMachines()
        {
            var machines = machineStore.GetMachines();
            var configurationMachines = configurationStore.GetConfiguration().Machines;
            int machinesControlled = 0;

            var autoStartMachines = machines
                .Where(a => configurationMachines.Any(b => b.Uuid == a.Uuid && b.AutoStart))
                .ToArray();

            if (autoStartMachines.Length > 0)
            {
                foreach (var machine in autoStartMachines)
                {
                    if (!machine.IsPoweredOff)
                    {
                        logger.LogInformation(
                            $"Skipping auto-start of machine \"{machine.Uuid}\", state: {machine.Metadata.State}"
                        );
                        continue;
                    }

                    logger.LogInformation($"Auto-starting machine \"{machine.Uuid}\"");

                    if (!machineController.StartMachineHeadless(machine))
                    {
                        logger.LogError($"Failed to start machine \"{machine.Uuid}\"");
                    }

                    // We want to include failed machines so the state is updated (likely to Aborted)
                    machinesControlled++;
                }
            }
            else
            {
                logger.LogInformation("No machines found to auto-start");
            }

            return machinesControlled > 0;
        }

        public void StopMachines()
        {
            var machines = machineStore.GetMachines();
            var configurationMachines = configurationStore.GetConfiguration().Machines;

            var monitoredMachines = machines
                .Where(a => configurationMachines.Any(b => b.Uuid == a.Uuid))
                .ToArray();

            foreach (var machine in monitoredMachines)
            {
                var configuration = configurationMachines.FirstOrDefault(c => c.Uuid == machine.Uuid);
                if (configuration == null)
                {
                    continue;
                }

                if (configuration.SaveState)
                {
                    logger.LogInformation($"Saving state of \"{machine.Name}\"");

                    if (!machineController.SaveState(machine))
                    {
                        logger.LogError($"Failed to start machine \"{machine.Name}\"");
                    }
                }
                else
                {
                    logger.LogInformation($"Powering off \"{machine.Name}\"");

                    const int waitLimit = 90000;

                    if (!machineController.AcpiPowerOff(machine, waitLimit, () => logger.LogDebug($"Waiting for power off")))
                    {
                        logger.LogError($"Failed to power off \"{machine.Name}\"");
                    }
                }
            }
        }
    }
}