using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace TrayApp
{
    public class NotifyIconManager : IDisposable
    {
        private readonly ILogger<NotifyIconManager> logger;

        public NotifyIcon NotifyIcon { get; private set; }

        public NotifyIconManager(ILogger<NotifyIconManager> logger)
        {
            logger.LogTrace(".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.NotifyIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.TrayIcon,
                Text = Properties.Resources.TrayTitle,
                Visible = false,
            };
        }

        public void ShowIcon() => NotifyIcon.Visible = true;

        public void HideIcon() => NotifyIcon.Visible = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            logger.LogTrace($"Dispose({disposing})");

            if (disposing && NotifyIcon != null)
            {
                NotifyIcon.Visible = false;
                NotifyIcon.Dispose();
                NotifyIcon = null;
            }
        }
    }
}