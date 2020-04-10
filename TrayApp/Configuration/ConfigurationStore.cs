using Microsoft.Extensions.Logging;
using System;
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
                var configuration = configurationReader.ReadConfiguration() ?? new TrayConfiguration(
                    LogLevelConfigurationManager.DefaultLevel
                );
                SetConfiguration(configuration);
            }
        }

        internal void SetConfiguration(TrayConfiguration newConfiguration)
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

            if (newConfiguration?.Machines != null)
            {
                foreach (var newMachine in newConfiguration.Machines)
                {
                    if (oldConfiguration == null)
                    {
                        logger.LogDebug($" - Machine added: \"{newMachine.Uuid}\"");
                        continue;
                    }

                    var oldMachine = oldConfiguration.Machines.FirstOrDefault(m => m.Uuid == newMachine.Uuid);
                    if (oldMachine == null)
                    {
                        logger.LogDebug($" - Machine added: \"{newMachine.Uuid}\"");
                        continue;
                    }

                    if (!oldMachine.Equals(newMachine))
                    {
                        logger.LogDebug($" - Machine changed: \"{newMachine.Uuid}\"");
                        logger.LogDebug("     Old: {machine}", new
                        {
                            oldMachine.Uuid,
                            oldMachine.AutoStart,
                            oldMachine.SaveState,
                        });
                        logger.LogDebug("     New: {machine}", new
                        {
                            newMachine.Uuid,
                            newMachine.AutoStart,
                            newMachine.SaveState,
                        });
                    }
                }
            }

            if (oldConfiguration?.Machines != null)
            {
                foreach (var oldMachine in oldConfiguration.Machines)
                {
                    if (!newConfiguration.Machines.Any(m => m.Uuid == oldMachine.Uuid))
                    {
                        logger.LogDebug($" - Machine removed: \"{oldMachine.Uuid}\"");
                    }
                }
            }
        }
    }
}