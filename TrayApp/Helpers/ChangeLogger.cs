using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TrayApp.Configuration;
using TrayApp.State;

namespace TrayApp.Helpers
{
    public class ChangeLogger
    {
        private readonly ILogger<ChangeLogger> logger;

        public ChangeLogger(ILogger<ChangeLogger> logger, AppState appState)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (appState == null)
            {
                throw new ArgumentNullException(nameof(appState));
            }

            appState.OnConfigurationChange += AppState_OnConfigurationChange;
            appState.OnMachineConfigurationChange += Machines_OnMachineConfigurationChange;
            appState.OnMachineStateChange += Machines_OnMachineStateChange;
        }

        private void AppState_OnConfigurationChange(object sender, ConfigurationChangeEventArgs e)
        {
            logger.LogDebug("Configuration changes");
            DumpChanges(e.PreviousConfiguration, e.NewConfiguration);
        }

        private void Machines_OnMachineStateChange(object sender, MachineStateChangeEventArgs e)
        {
            logger.LogDebug("Machine state changes");
            DumpChanges(e.PreviousMachines.ToArray(), e.NewMachines.ToArray());
        }

        private void Machines_OnMachineConfigurationChange(object sender, MachineConfigurationChangeEventArgs e)
        {
            logger.LogDebug("Machine configuration changes");
            DumpChanges(e.PreviousMachines.ToArray(), e.NewMachines.ToArray());
        }

        private void DumpChanges(AppConfiguration oldConfiguration, AppConfiguration newConfiguration)
        {
            if (newConfiguration == null)
            {
                throw new ArgumentNullException(nameof(newConfiguration));
            }

            logger.LogDebug($" - Old: {(oldConfiguration == null ? "null" : oldConfiguration.ToString())}");
            logger.LogDebug($" - New: {newConfiguration}");
        }

        internal void DumpChanges(IUuidContainer[] oldMachines, IUuidContainer[] newMachines)
        {
            if (newMachines == null)
            {
                throw new ArgumentNullException(nameof(newMachines));
            }

            if (oldMachines == null)
            {
                oldMachines = Array.Empty<IUuidContainer>();
            }

            var added = newMachines.Except(oldMachines, new UuidEqualityComparer());
            var removed = oldMachines.Except(newMachines, new UuidEqualityComparer());

            foreach (var machine in added)
            {
                logger.LogDebug($" - Added {machine}");
            }

            foreach (var machine in removed)
            {
                logger.LogDebug($" - Removed {machine}");
            }

            foreach (var newMachine in newMachines)
            {
                var oldMachine = Array.Find(oldMachines, m => m.Uuid == newMachine.Uuid);
                if (oldMachine?.Equals(newMachine) == false)
                {
                    logger.LogDebug(" - Changed");
                    logger.LogDebug($"     Old {oldMachine}");
                    logger.LogDebug($"     New {newMachine}");
                }
            }
        }
    }
}