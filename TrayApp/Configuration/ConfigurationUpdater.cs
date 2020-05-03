using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Windows.Forms;
using TrayApp.Forms;
using TrayApp.State;
using TrayApp.VirtualMachine;

namespace TrayApp.Configuration
{
    public class ConfigurationUpdater : IDisposable
    {
        private ConfigureForm configurationForm;
        private readonly ILogger<ConfigurationUpdater> logger;
        private readonly AppState appState;
        private readonly IMachineLocator machineLocator;
        private readonly IConfigurationWriter configurationWriter;

        public ConfigurationUpdater(
            ILogger<ConfigurationUpdater> logger,
            AppState appState,
            IMachineLocator machineLocator,
            IConfigurationWriter configurationWriter
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
            this.machineLocator = machineLocator ?? throw new ArgumentNullException(nameof(machineLocator));
            this.configurationWriter = configurationWriter ?? throw new ArgumentNullException(nameof(configurationWriter));
        }

        public void ShowConfigurationForm()
        {
            if (configurationForm != null)
            {
                configurationForm.Activate();
                return;
            }

            appState.UpdateConfiguration();

            configurationForm = new ConfigureForm(appState.Configuration, machineLocator.ListMachines());

            if (configurationForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    configurationWriter.WriteConfiguration(configurationForm.UpdatedConfiguration);
                }
                catch (Exception e) when (e is InvalidOperationException || e is DirectoryNotFoundException)
                {
                    logger.LogError(e, $"Failed to write configuration {new { Error = e.Message }}");

                    MessageBox.Show(
                        $"Failed to write configuration, {e.Message}.",
                        Properties.Resources.TrayTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }

                appState.UpdateConfiguration();
            }

            configurationForm.Dispose();
            configurationForm = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                configurationForm?.Dispose();
                configurationForm = null;
            }
        }
    }
}