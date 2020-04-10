using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Windows.Forms;
using TrayApp.Configuration;
using TrayApp.Forms;
using TrayApp.VirtualMachine;

namespace TrayApp.Menu.Handler
{
    public class ConfigureMenuHandler : IMenuHandler, IDisposable
    {
        private readonly ILogger<ConfigureMenuHandler> logger;
        private readonly ConfigurationStore configurationStore;
        private readonly ILocatorService locatorService;
        private readonly IConfigurationWriter configurationWriter;
        private ToolStripMenuItem menuItem;

        public ConfigureMenuHandler(
            ILogger<ConfigureMenuHandler> logger,
            ConfigurationStore configurationStore,
            ILocatorService locatorService,
            IConfigurationWriter configurationWriter
        )
        {
            logger.LogTrace(".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));
            this.locatorService = locatorService ?? throw new ArgumentNullException(nameof(locatorService));
            this.configurationWriter = configurationWriter ?? throw new ArgumentNullException(nameof(configurationWriter));
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

        private void OnConfigure(object sender, EventArgs e)
        {
            configurationStore.UpdateConfiguration();

            using var form = new ConfigureForm(configurationStore.GetConfiguration(), locatorService.LocateMachines(false));

            var result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (form.UpdatedConfiguration.Equals(configurationStore.GetConfiguration()))
                {
                    MessageBox.Show(
                        $"Nothing changed.",
                        Properties.Resources.TrayTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                try
                {
                    configurationWriter.WriteConfiguration(form.UpdatedConfiguration);
                }
                catch (Exception exception) when (exception is InvalidOperationException ||
                                                 exception is DirectoryNotFoundException)
                {
                    logger.LogError(exception, "Failed to write configuration");

                    MessageBox.Show(
                        $"Failed to write configuration, {exception.Message}.",
                        Properties.Resources.TrayTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }

                configurationStore.UpdateConfiguration();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            logger.LogTrace($"Dispose({disposing})");

            if (disposing)
            {
                menuItem?.Dispose();
                menuItem = null;
            }
        }
    }
}