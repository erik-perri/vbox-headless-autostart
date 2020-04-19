using System;
using System.Collections.ObjectModel;
using TrayApp.Helpers;
using TrayApp.Logging;

namespace TrayApp.Configuration
{
    public class ConfigurationFactory
    {
        private readonly StartupManager startupManager;

        public ConfigurationFactory(StartupManager startupManager)
        {
            this.startupManager = startupManager ?? throw new ArgumentNullException(nameof(startupManager));
        }

        public AppConfiguration GetDefaultAppConfiguration()
        {
            return new AppConfiguration(
                LogLevelConfigurationManager.DefaultLevel,
                false,
                startupManager.IsEnabled(),
                new ReadOnlyCollection<MachineConfiguration>(Array.Empty<MachineConfiguration>())
            );
        }

        public static MachineConfiguration GetDefaultMachineConfiguration(string uuid)
        {
            return new MachineConfiguration(uuid, true, true, true, true);
        }
    }
}