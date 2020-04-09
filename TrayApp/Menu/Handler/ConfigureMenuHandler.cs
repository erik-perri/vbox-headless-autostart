using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace TrayApp.Menu.Handler
{
    public class ConfigureMenuHandler : IMenuHandler, IDisposable
    {
        private readonly ILogger<ConfigureMenuHandler> logger;
        private ToolStripMenuItem menuItem;

        public ConfigureMenuHandler(ILogger<ConfigureMenuHandler> logger)
        {
            logger.LogTrace(".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            logger.LogTrace("OnConfigure");
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