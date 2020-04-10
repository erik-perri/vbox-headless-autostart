using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TrayApp.Helpers;
using TrayApp.Logging;

namespace TrayApp.Configuration
{
    public class ConfigurationStore
    {
        private readonly ILogger<ConfigurationStore> logger;
        private TrayConfiguration configuration;
        private readonly IConfigurationReader configurationReader;

        public event EventHandler OnConfigurationChange;

        public ConfigurationStore(ILogger<ConfigurationStore> logger, IConfigurationReader reader)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configurationReader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public TrayConfiguration GetConfiguration()
        {
            return configuration;
        }

        public void UpdateConfiguration()
        {
            var configuration = configurationReader.ReadConfiguration();

            if (configuration == null)
            {
                configuration = new TrayConfiguration()
                {
                    LogLevel = LogLevelConfigurationManager.DefaultLevel,
                    ShowKeepAwakeMenu = false,
                    Machines = new ReadOnlyCollection<MachineConfiguration>(Array.Empty<MachineConfiguration>()),
                };
            }

            SetConfiguration(configuration);
        }

        private void SetConfiguration(TrayConfiguration newConfiguration)
        {
            if (newConfiguration == null)
            {
                throw new ArgumentNullException(nameof(newConfiguration));
            }

            var previousConfiguration = configuration;
            configuration = newConfiguration;

            if (!newConfiguration.Equals(previousConfiguration))
            {
                DumpChanges(previousConfiguration, newConfiguration);

                OnConfigurationChange?.Invoke(this, EventArgs.Empty);
            }
        }

        private void DumpChanges(TrayConfiguration oldConfiguration, TrayConfiguration newConfiguration)
        {
            if (newConfiguration == null)
            {
                throw new ArgumentNullException(nameof(newConfiguration));
            }

            logger.LogDebug("Configuration store changed");

            if (oldConfiguration?.LogLevel != newConfiguration?.LogLevel)
            {
                var newValue = FormattableString.Invariant($"\"{newConfiguration.LogLevel}\"");
                var oldValue = oldConfiguration == null
                    ? "null"
                    : FormattableString.Invariant($"\"{oldConfiguration.LogLevel}\"");

                logger.LogDebug($" - LogLevel changed: {oldValue} -> {newValue}");
            }

            if (oldConfiguration?.ShowKeepAwakeMenu != newConfiguration?.ShowKeepAwakeMenu)
            {
                var newValue = FormattableString.Invariant($"\"{newConfiguration.ShowKeepAwakeMenu}\"");
                var oldValue = oldConfiguration == null
                    ? "null"
                    : FormattableString.Invariant($"\"{oldConfiguration.ShowKeepAwakeMenu}\"");

                logger.LogDebug($" - ShowKeepAwakeMenu changed: {oldValue} -> {newValue}");
            }

            DumpMachineListChanges(oldConfiguration?.Machines.ToArray(), newConfiguration.Machines.ToArray());
        }

        private void DumpMachineListChanges(MachineConfiguration[] oldMachines, MachineConfiguration[] newMachines)
        {
            if (newMachines == null)
            {
                throw new ArgumentNullException(nameof(newMachines));
            }

            if (oldMachines == null)
            {
                oldMachines = Array.Empty<MachineConfiguration>();
            }

            logger.LogDebug(" - Machines changed");

            var added = newMachines.Except(oldMachines, new UuidEqualityComparer());
            var removed = oldMachines.Except(newMachines, new UuidEqualityComparer());

            foreach (var machine in added)
            {
                logger.LogDebug($"    - Added:   {new { machine.Uuid, machine.AutoStart, machine.SaveState }}");
            }

            foreach (var machine in removed)
            {
                logger.LogDebug($"    - Removed: {new { machine.Uuid, machine.AutoStart, machine.SaveState }}");
            }

            foreach (var newMachine in newMachines)
            {
                var oldMachine = Array.Find(oldMachines, m => m.Uuid == newMachine.Uuid);
                if (oldMachine?.Equals(newMachine) == false)
                {
                    logger.LogDebug($"    - Changed:");
                    logger.LogDebug($"        Old: {new { oldMachine.Uuid, oldMachine.AutoStart, oldMachine.SaveState }}");
                    logger.LogDebug($"        New: {new { newMachine.Uuid, newMachine.AutoStart, newMachine.SaveState }}");
                }
            }
        }
    }
}