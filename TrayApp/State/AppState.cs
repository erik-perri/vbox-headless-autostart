using System;
using System.Collections.ObjectModel;
using System.Linq;
using TrayApp.Configuration;
using TrayApp.VirtualMachine;

namespace TrayApp.State
{
    public class AppState
    {
        private IMachineMetadata[] machines;
        private readonly IMachineLocator machineLocator;
        private readonly IConfigurationReader configurationReader;
        private readonly ConfigurationFactory configurationFactory;

        public AppConfiguration Configuration { get; private set; }

        public event EventHandler<ConfigurationChangeEventArgs> OnConfigurationChange;

        public event EventHandler<MachineConfigurationChangeEventArgs> OnMachineConfigurationChange;

        public event EventHandler<MachineStateChangeEventArgs> OnMachineStateChange;

        public AppState(
            IMachineLocator machineLocator,
            IConfigurationReader configurationReader,
            ConfigurationFactory configurationFactory
        )
        {
            this.machineLocator = machineLocator ?? throw new ArgumentNullException(nameof(machineLocator));
            this.configurationReader = configurationReader ?? throw new ArgumentNullException(nameof(configurationReader));
            this.configurationFactory = configurationFactory ?? throw new ArgumentNullException(nameof(configurationFactory));

            machines = Array.Empty<IMachineMetadata>();
            Configuration = null;
        }

        public void UpdateConfiguration()
        {
            var newConfiguration = configurationReader.ReadConfiguration()
                            ?? configurationFactory.GetDefaultAppConfiguration();

            if (!newConfiguration.Equals(Configuration))
            {
                var oldConfiguration = Configuration;

                Configuration = newConfiguration;

                OnConfigurationChange?.Invoke(
                    this,
                    new ConfigurationChangeEventArgs(oldConfiguration, newConfiguration)
                );

                var oldMachines = oldConfiguration?.Machines ??
                    new ReadOnlyCollection<MachineConfiguration>(Array.Empty<MachineConfiguration>());

                if (!newConfiguration.Machines.OrderBy(c => c.Uuid).SequenceEqual(oldMachines.OrderBy(c => c.Uuid)))
                {
                    OnMachineConfigurationChange?.Invoke(
                        this,
                        new MachineConfigurationChangeEventArgs(oldMachines, newConfiguration.Machines)
                    );
                }
            }
        }

        public void UpdateMachines()
        {
            var newMachines = machineLocator.ListMachines();
            // If we get null the VirtualBox instance was invalid.  There is no point updating the machine state if we
            // can't obtain it.
            if (newMachines == null)
            {
                return;
            }

            if (!newMachines.OrderBy(m => m.Uuid).SequenceEqual(machines.OrderBy(m => m.Uuid)))
            {
                var newCollection = newMachines;
                var oldCollection = machines;

                machines = newCollection;

                OnMachineStateChange?.Invoke(this, new MachineStateChangeEventArgs(oldCollection, newCollection));
            }
        }

        public IMachineMetadata[] GetMachines()
        {
            return machines.ToArray();
        }

        public IMachineMetadata[] GetMachines(Func<IMachineMetadata, MachineConfiguration, bool> predicate)
        {
            return machines.Where(machine =>
            {
                var configuration = Configuration.Machines.FirstOrDefault(c => c.Uuid == machine.Uuid);

                return predicate(machine, configuration);
            }).ToArray();
        }

        public bool HasMachines(Func<IMachineMetadata, MachineConfiguration, bool> predicate)
        {
            return machines.Any(machine =>
            {
                var configuration = Configuration.Machines.FirstOrDefault(c => c.Uuid == machine.Uuid);

                return predicate(machine, configuration);
            });
        }
    }
}