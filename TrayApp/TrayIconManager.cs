using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TrayApp.Menu;

namespace TrayApp
{
    public class TrayIconManager : IDisposable
    {
        private readonly NotifyIcon trayIcon;
        private readonly ILogger<TrayIconManager> logger;

        public TrayIconManager(ILogger<TrayIconManager> logger)
        {
            logger.LogTrace(".ctor");

            this.logger = logger;
            trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.TrayIcon,
                Text = Properties.Resources.TrayTitle,
                Visible = true,
            };
        }

        public void SetContextMenu(TrayContextMenuStrip contextMenu)
        {
            trayIcon.ContextMenuStrip = contextMenu;
        }

        public void Hide()
        {
            trayIcon.Visible = false;
        }

        public void Show()
        {
            trayIcon.Visible = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            logger.LogTrace($"Dispose({disposing})");

            if (disposing && trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon?.Dispose();
            }
        }
    }
}