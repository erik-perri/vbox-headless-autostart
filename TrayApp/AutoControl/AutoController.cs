using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TrayApp.Configuration;
using TrayApp.VirtualMachine;

namespace TrayApp.AutoControl
{
    public class AutoController
    {
        private readonly ILogger<AutoController> logger;
        private readonly MachineStore machineStore;
        private readonly ConfigurationStore configurationStore;
        private readonly IMachineController machineController;

        public AutoController(
            ILogger<AutoController> logger,
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
                            $"Skipping auto-start {new { machine.Uuid, machine.Name, machine.Metadata.State }}"
                        );
                        continue;
                    }

                    logger.LogInformation($"Auto-starting {new { machine.Uuid, machine.Name }}");

                    if (!machineController.StartMachineHeadless(machine))
                    {
                        logger.LogError($"Failed to start {new { machine.Uuid, machine.Name }}");
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Simplification", "RCS1021:Simplify lambda expression.", Justification = "Line too long"
        )]
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
                    logger.LogInformation($"Saving state {new { machine.Uuid, machine.Name }}");

                    if (!machineController.SaveState(machine))
                    {
                        logger.LogError($"Failed to save state {new { machine.Uuid, machine.Name }}");
                    }
                }
                else
                {
                    logger.LogInformation($"Powering off {new { machine.Uuid, machine.Name }}");

                    const int waitLimit = 90000;

                    if (!machineController.AcpiPowerOff(machine, waitLimit, () =>
                    {
                        logger.LogDebug($"Waiting for power off {new { machine.Uuid, machine.Name }}");
                    }))
                    {
                        logger.LogError($"Failed to power off {new { machine.Uuid, machine.Name }}");
                    }
                }
            }
        }
    }
}