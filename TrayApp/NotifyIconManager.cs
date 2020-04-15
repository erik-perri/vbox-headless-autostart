using System;
using System.Windows.Forms;

namespace TrayApp
{
    public class NotifyIconManager : IDisposable
    {
        public NotifyIcon NotifyIcon { get; private set; }

        public NotifyIconManager()
        {
            NotifyIcon = new NotifyIcon()
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
            if (disposing && NotifyIcon != null)
            {
                NotifyIcon.Visible = false;
                NotifyIcon.Dispose();
                NotifyIcon = null;
            }
        }
    }
}