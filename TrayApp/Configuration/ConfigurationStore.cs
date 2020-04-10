using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TrayApp.Logging;

namespace TrayApp.Configuration
{
    public class ConfigurationStore
    {
        private readonly object configurationLock = new object();
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
            lock (configurationLock)
            {
                return configuration;
            }
        }

        public void UpdateConfiguration()
        {
            lock (configurationLock)
            {
                var configuration = configurationReader.ReadConfiguration() ?? new TrayConfiguration()
                {
                    LogLevel = LogLevelConfigurationManager.DefaultLevel,
                    ShowKeepAwakeMenu = false,
                    Machines = new ReadOnlyCollection<MachineConfiguration>(Array.Empty<MachineConfiguration>()),
                };
                SetConfiguration(configuration);
            }
        }

        private void SetConfiguration(TrayConfiguration newConfiguration)
        {
            if (newConfiguration == null)
            {
                throw new ArgumentNullException(nameof(newConfiguration));
            }

            lock (configurationLock)
            {
                var previousConfiguration = configuration;
                configuration = newConfiguration;

                if (!newConfiguration.Equals(previousConfiguration))
                {
                    DumpChanges(previousConfiguration, newConfiguration);

                    OnConfigurationChange?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void DumpChanges(TrayConfiguration oldConfiguration, TrayConfiguration newConfiguration)
        {
            logger.LogDebug("Configuration modified");

            if (oldConfiguration?.LogLevel != newConfiguration?.LogLevel)
            {
                logger.LogDebug(
                    $" - Log level changed: \"{oldConfiguration?.LogLevel}\" -> \"{newConfiguration?.LogLevel}\""
                );
            }

            if (oldConfiguration?.ShowKeepAwakeMenu != newConfiguration?.ShowKeepAwakeMenu)
            {
                logger.LogDebug(
                    $" - Show keep awake menu changed: \"{oldConfiguration?.ShowKeepAwakeMenu}\" -> \"{newConfiguration?.ShowKeepAwakeMenu}\""
                );
            }

            if (newConfiguration?.Machines != null)
            {
                foreach (var newMachine in newConfiguration.Machines)
                {
                    var newLog = new
                    {
                        newMachine.Uuid,
                        newMachine.AutoStart,
                        newMachine.SaveState,
                    };

                    if (oldConfiguration == null)
                    {
                        logger.LogDebug($" - Added: {newLog}");
                        continue;
                    }

                    var oldMachine = oldConfiguration.Machines.FirstOrDefault(m => m.Uuid == newMachine.Uuid);
                    if (oldMachine == null)
                    {
                        logger.LogDebug($" - Added: {newLog}");
                        continue;
                    }

                    if (!oldMachine.Equals(newMachine))
                    {
                        var oldLog = new
                        {
                            oldMachine.Uuid,
                            oldMachine.AutoStart,
                            oldMachine.SaveState,
                        };

                        logger.LogDebug($" - Changed:");
                        logger.LogDebug($"     Old: {oldLog}");
                        logger.LogDebug($"     New: {newLog}");
                    }
                }
            }

            if (oldConfiguration?.Machines != null)
            {
                foreach (var oldMachine in oldConfiguration.Machines)
                {
                    if (!newConfiguration.Machines.Any(m => m.Uuid == oldMachine.Uuid))
                    {
                        var oldLog = new
                        {
                            oldMachine.Uuid,
                            oldMachine.AutoStart,
                            oldMachine.SaveState,
                        };

                        logger.LogDebug($" - Removed: {oldLog}");
                    }
                }
            }
        }
    }
}