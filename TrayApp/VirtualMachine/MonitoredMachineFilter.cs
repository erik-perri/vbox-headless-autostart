using CommonLib.VirtualMachine;
using System;
using System.Linq;
using TrayApp.Configuration;

namespace TrayApp.VirtualMachine
{
    public class MonitoredMachineFilter : IMachineFilter
    {
        private readonly ConfigurationStore configurationStore;

        public MonitoredMachineFilter(ConfigurationStore configurationStore)
        {
            this.configurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));
        }

        public bool IncludeMachine(string uuid)
        {
            var configuration = configurationStore.GetConfiguration();
            if (configuration == null)
            {
                throw new InvalidOperationException("Configuration not initialized");
            }

            return configuration.Machines.Any(m => m.Uuid == uuid);
        }
    }
}