using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Windows.Forms;
using TrayApp.Configuration;
using TrayApp.Forms;
using TrayApp.State;
using TrayApp.VirtualMachine;

namespace TrayApp.Menu.Handler
{
    public class ConfigureMenuHandler : IMenuHandler, IDisposable
    {
        private readonly ILogger<ConfigureMenuHandler> logger;
        private readonly AppState appState;
        private readonly IMachineLocator machineLocator;
        private readonly IConfigurationWriter configurationWriter;
        private ToolStripMenuItem menuItem;
        private ConfigureForm configurationForm;

        public ConfigureMenuHandler(
            ILogger<ConfigureMenuHandler> logger,
            AppState appState,
            IMachineLocator machineLocator,
            IConfigurationWriter configurationWriter,
            NotifyIconManager notifyIconManager
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));
            this.machineLocator = machineLocator ?? throw new ArgumentNullException(nameof(machineLocator));
            this.configurationWriter = configurationWriter ?? throw new ArgumentNullException(nameof(configurationWriter));

            if (notifyIconManager is null)
            {
                throw new ArgumentNullException(nameof(notifyIconManager));
            }

            notifyIconManager.NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        public int GetSortOrder()
        {
            return 0;
        }

        public ToolStripItem[] CreateMenuItems()
        {
            menuItem?.Dispose();
            menuItem = new ToolStripMenuItem("&Configure", Properties.Resources.Settings, OnConfigure);

            return new ToolStripItem[] { menuItem };
        }

        private void ShowConfigurationForm()
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
                catch (Exception e) when (e is InvalidOperationException
                                        || e is DirectoryNotFoundException)
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

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowConfigurationForm();
        }

        private void OnConfigure(object sender, EventArgs eventArgs)
        {
            ShowConfigurationForm();
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
                menuItem?.Dispose();
                menuItem = null;

                configurationForm?.Dispose();
                configurationForm = null;
            }
        }
    }
}