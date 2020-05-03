using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using TrayApp.Configuration;
using TrayApp.Helpers;
using TrayApp.Menu;
using TrayApp.State;

namespace TrayApp
{
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly object updateLock = new object();
        private readonly ILogger<TrayApplicationContext> logger;
        private readonly TrayContextMenuStrip contextMenu;

        public TrayApplicationContext(
            ILogger<TrayApplicationContext> logger,
            TrayContextMenuStrip contextMenu,
            NotifyIconManager notifyIconManager,
            AppState appState,
            StartupManager startupManager,
            ConfigurationUpdater configurationUpdater
        )
        {
            if (notifyIconManager == null)
            {
                throw new ArgumentNullException(nameof(notifyIconManager));
            }

            if (appState == null)
            {
                throw new ArgumentNullException(nameof(appState));
            }

            if (startupManager == null)
            {
                throw new ArgumentNullException(nameof(startupManager));
            }

            if (configurationUpdater == null)
            {
                throw new ArgumentNullException(nameof(configurationUpdater));
            }

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextMenu = contextMenu ?? throw new ArgumentNullException(nameof(contextMenu));

            notifyIconManager.NotifyIcon.ContextMenuStrip = contextMenu;

            appState.OnMachineStateChange += (object _, MachineStateChangeEventArgs __) => UpdateContextMenu();

            appState.OnConfigurationChange += (object _, ConfigurationChangeEventArgs e) =>
            {
                if (e.NewConfiguration.StartWithWindows != startupManager.IsEnabled())
                {
                    if (e.NewConfiguration.StartWithWindows)
                    {
                        startupManager.EnableStartup();
                    }
                    else
                    {
                        startupManager.DisableStartup();
                    }
                }

                if (e.NewConfiguration.ShowTrayIcon)
                {
                    notifyIconManager.ShowIcon();
                }
                else
                {
                    notifyIconManager.HideIcon();
                }

                CreateContextMenu();
            };

            if (appState.Configuration.ShowTrayIcon)
            {
                notifyIconManager.ShowIcon();
            }
            else if (!Program.IsAutoStarting())
            {
                // If the program is not auto-starting and the tray icon is not visible the user launched the
                // application manually and it was not already running.  We will show the configuration under the
                // assumption they are looking for it.
                configurationUpdater.ShowConfigurationForm();
            }

            CreateContextMenu();
        }

        private void CreateContextMenu()
        {
            lock (updateLock)
            {
                logger.LogTrace($"Creating context menu {new { contextMenu.InvokeRequired }}");

                if (contextMenu.InvokeRequired)
                {
                    contextMenu.Invoke(new MethodInvoker(delegate { contextMenu.CreateContextMenu(); }));
                    return;
                }

                contextMenu.CreateContextMenu();
            }
        }

        private void UpdateContextMenu()
        {
            lock (updateLock)
            {
                logger.LogTrace($"Updating context menu {new { contextMenu.InvokeRequired }}");

                if (contextMenu.InvokeRequired)
                {
                    contextMenu.Invoke(new MethodInvoker(delegate { contextMenu.UpdateContextMenu(); }));
                    return;
                }

                contextMenu.UpdateContextMenu();
            }
        }
    }
}