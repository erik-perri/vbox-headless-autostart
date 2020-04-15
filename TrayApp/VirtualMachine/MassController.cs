using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TrayApp.State;

namespace TrayApp.VirtualMachine
{
    public class MassController
    {
        private readonly ILogger<MassController> logger;
        private readonly AppState appState;
        private readonly IMachineController machineController;

        public MassController(
            ILogger<MassController> logger,
            AppState appState,
            IMachineController machineController
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
            this.machineController = machineController ?? throw new ArgumentNullException(nameof(machineController));
        }

        public void StartAll()
        {
            var machines = appState.Machines;
            var configurationMachines = appState.Configuration.Machines;

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

                    machineController.StartMachine(machine, true);
                }
            }
            else
            {
                logger.LogInformation("No machines found to auto-start");
            }
        }

        public void StopAll()
        {
            var machines = appState.Machines;
            var configurationMachines = appState.Configuration.Machines;

            foreach (var machine in machines)
            {
                var configuration = configurationMachines.FirstOrDefault(c => c.Uuid == machine.Uuid);
                if (configuration == null)
                {
                    continue;
                }

                if (!machine.IsPoweredOn)
                {
                    logger.LogInformation(
                        $"Skipping auto-stop {machine}"
                    );
                    continue;
                }

                if (configuration.SaveState)
                {
                    machineController.SaveState(machine);
                }
                else if (!machineController.AcpiPowerOff(
                    machine,
                    90000,
                    () => logger.LogDebug($"Waiting for power off {machine}")
                ))
                {
                    logger.LogError($"Failed to power off {machine}");
                }
            }
        }
    }
}