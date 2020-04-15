using CommonLib.Configuration;
using CommonLib.Helpers;
using CommonLib.VirtualMachine;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TrayApp.Helpers;
using TrayApp.Logging;

namespace TrayApp.State
{
    public class AppState
    {
        private readonly ILogger<AppState> logger;
        private readonly IMachineLocator machineLocator;
        private readonly IConfigurationReader configurationReader;

        public AppConfiguration Configuration { get; private set; }

        public ReadOnlyCollection<IMachineMetadata> Machines { get; private set; }

        public event EventHandler OnConfigurationChange;

        public event EventHandler OnMachineListChange;

        public event EventHandler OnMachineStateChange;

        public AppState(
            ILogger<AppState> logger,
            IMachineLocator machineLocator,
            IConfigurationReader configurationReader
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.machineLocator = machineLocator ?? throw new ArgumentNullException(nameof(machineLocator));
            this.configurationReader = configurationReader ?? throw new ArgumentNullException(nameof(configurationReader));

            Machines = new ReadOnlyCollection<IMachineMetadata>(Array.Empty<IMachineMetadata>());
        }

        public void UpdateConfiguration()
        {
            var configuration = configurationReader.ReadConfiguration();
            if (configuration == null)
            {
                configuration = new AppConfiguration(
                    LogLevelConfigurationManager.DefaultLevel,
                    false,
                    new ReadOnlyCollection<MachineConfiguration>(Array.Empty<MachineConfiguration>())
                );
            }

            if (!configuration.Equals(Configuration))
            {
                DumpChanges(Configuration, configuration);

                Configuration = configuration;

                OnConfigurationChange?.Invoke(this, EventArgs.Empty);
            }
        }

        internal void UpdateMachines()
        {
            var vboxMachines = machineLocator.ListMachines() ?? Array.Empty<IMachineMetadata>();
            var monitoredMachines = vboxMachines.Where(
                vboxMachine => Configuration.Machines.Any(
                    confMachine => confMachine.Uuid == vboxMachine.Uuid
                )
            );

            if (!monitoredMachines.OrderBy(m => m.Uuid).SequenceEqual(Machines.OrderBy(m => m.Uuid)))
            {
                var oldMachines = Machines.ToArray();
                var newMachines = monitoredMachines.ToArray();

                logger.LogDebug("Machine metadata changes");

                DumpChanges(oldMachines, newMachines);

                Machines = new ReadOnlyCollection<IMachineMetadata>(newMachines);

                if (WasMachineListChanged(oldMachines, newMachines))
                {
                    OnMachineListChange?.Invoke(this, EventArgs.Empty);
                }

                OnMachineStateChange?.Invoke(this, EventArgs.Empty);
            }
        }

        private static bool WasMachineListChanged(IMachineMetadata[] oldMachines, IMachineMetadata[] newMachines)
        {
            if (oldMachines == null)
            {
                throw new ArgumentNullException(nameof(oldMachines));
            }

            if (newMachines == null)
            {
                throw new ArgumentNullException(nameof(newMachines));
            }

            return oldMachines == null
                || newMachines.Except(oldMachines, new UuidEqualityComparer()).Any()
                || oldMachines.Except(newMachines, new UuidEqualityComparer()).Any();
        }

        private void DumpChanges(AppConfiguration oldConfiguration, AppConfiguration newConfiguration)
        {
            if (newConfiguration == null)
            {
                throw new ArgumentNullException(nameof(newConfiguration));
            }

            logger.LogDebug("Configuration changes");

            logger.LogDebug($" - Old: {oldConfiguration}");
            logger.LogDebug($" - New: {newConfiguration}");

            logger.LogDebug("Configuration machine changes");

            DumpChanges(oldConfiguration?.Machines.ToArray(), newConfiguration.Machines.ToArray());
        }

        private void DumpChanges(IUuidContainer[] oldMachines, IUuidContainer[] newMachines)
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
                logger.LogDebug($"    - Added {machine}");
            }

            foreach (var machine in removed)
            {
                logger.LogDebug($"    - Removed {machine}");
            }

            foreach (var newMachine in newMachines)
            {
                var oldMachine = Array.Find(oldMachines, m => m.Uuid == newMachine.Uuid);
                if (oldMachine?.Equals(newMachine) == false)
                {
                    logger.LogDebug("    - Changed");
                    logger.LogDebug($"        Old {oldMachines}");
                    logger.LogDebug($"        New {newMachine}");
                }
            }
        }
    }
}