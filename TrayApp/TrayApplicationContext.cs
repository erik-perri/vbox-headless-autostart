using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using TrayApp.Menu;

namespace TrayApp
{
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly ILogger<TrayApplicationContext> logger;
        private readonly TrayIconManager iconManager;
        private readonly TrayContextMenuStrip contextMenu;

        public TrayApplicationContext(
            ILogger<TrayApplicationContext> logger,
            TrayIconManager iconManager,
            TrayContextMenuStrip contextMenu
        )
        {
            logger.LogTrace(".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextMenu = contextMenu ?? throw new ArgumentNullException(nameof(contextMenu));
            this.iconManager = iconManager ?? throw new ArgumentNullException(nameof(iconManager));

            iconManager.SetContextMenu(contextMenu);
            iconManager.Show();

            contextMenu.CreateContextMenu();
        }

        protected override void Dispose(bool disposing)
        {
            logger.LogTrace($"Dispose({disposing})");

            if (disposing)
            {
                contextMenu?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}