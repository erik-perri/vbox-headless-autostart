﻿using CommonLib.VirtualMachine;
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
                            $"Skipping auto-start {machine}"
                        );
                        continue;
                    }

                    logger.LogInformation($"Auto-starting {machine}");

                    if (!machineController.StartMachine(machine, true))
                    {
                        logger.LogError($"Failed to start {machine}");
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
                    logger.LogInformation($"Saving state {machine}");

                    if (!machineController.SaveState(machine))
                    {
                        logger.LogError($"Failed to save state {machine}");
                    }
                }
                else
                {
                    logger.LogInformation($"Powering off {machine}");

                    if (!machineController.AcpiPowerOff(machine, 90000, () =>
                    {
                        logger.LogDebug($"Waiting for power off {machine}");
                    }))
                    {
                        logger.LogError($"Failed to power off {machine}");
                    }
                }
            }
        }
    }
}