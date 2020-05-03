using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TrayApp.Configuration;
using TrayApp.State;

namespace TrayApp.VirtualMachine
{
    public class MassController
    {
        private readonly ILogger<MassController> logger;
        private readonly AppState appState;
        private readonly IMachineController machineController;

        public MassController(ILogger<MassController> logger, AppState appState, IMachineController machineController)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
            this.machineController = machineController ?? throw new ArgumentNullException(nameof(machineController));
        }

        public void StartAll(Func<IMachineMetadata, MachineConfiguration, bool> predicate)
        {
            foreach (var machine in appState.GetMachines(predicate))
            {
                if (!machine.IsPoweredOff)
                {
                    logger.LogInformation($"Skipping start {machine}");
                    continue;
                }

                machineController.Start(machine, true);
            }
        }

        public void StopAll(Func<IMachineMetadata, MachineConfiguration, bool> predicate)
        {
            foreach (var machine in appState.GetMachines(predicate))
            {
                if (!machine.IsPoweredOn)
                {
                    logger.LogInformation($"Skipping stop {machine}");
                    continue;
                }

                var configuration = appState.Configuration.Machines.FirstOrDefault(c => c.Uuid == machine.Uuid);
                if (configuration == null)
                {
                    continue;
                }

                if (configuration.SaveState)
                {
                    machineController.SaveState(machine);
                }
                else if (!machineController.AcpiPowerOff(machine, 90000))
                {
                    logger.LogError($"Failed to power off via ACPI {machine}");
                }
            }
        }
    }
}