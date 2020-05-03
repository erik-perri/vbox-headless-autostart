using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrayApp.Configuration;
using TrayApp.Configuration.FileLocator;
using TrayApp.Forms;
using TrayApp.Helpers;
using TrayApp.KeepAwake;
using TrayApp.Logging;
using TrayApp.Menu;
using TrayApp.Menu.Handler;
using TrayApp.Shutdown;
using TrayApp.State;
using TrayApp.VirtualMachine;
using TrayApp.VirtualMachine.VirtualBoxSdk;

namespace TrayApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace).AddNLog("NLog.config.xml"))

                .AddSingleton<AppState>()
                    .AddSingleton<ChangeLogger>()

                // Application context
                .AddSingleton<TrayApplicationContext>()
                    .AddSingleton<NotifyIconManager>()

                // Menu
                .AddSingleton<TrayContextMenuStrip>()
                    .AddSingleton(provider => new TrayHandlerCollection(
                        provider.GetServices<IMenuHandler>().OrderBy(m => m.GetSortOrder()).ToArray()
                    ))
                    .AddSingleton<IMenuHandler, ExitMenuHandler>()
                    .AddSingleton<IMenuHandler, ConfigureMenuHandler>()
                    .AddSingleton<IMenuHandler, MachineControlMenuHandler>()
                    .AddSingleton<IMenuHandler, KeepAwakeMenuHandler>()
                        .AddSingleton<KeepAwakeTask>()
                    .AddSingleton<IMenuHandler, MassControlMenuHandler>()

                // Virtual machines
                .AddSingleton<IMachineController, VirtualBoxInterface>()
                .AddSingleton<IMachineLocator, VirtualBoxInterface>()
                    .AddSingleton(p => new VirtualBoxProxyFactory(p))
                        .AddTransient<VirtualMachine.VirtualBoxSdk.Proxy.Version60.VirtualBoxProxy>()
                        .AddTransient<VirtualMachine.VirtualBoxSdk.Proxy.Version61.VirtualBoxProxy>()
                .AddSingleton<MachineStateUpdater>()
                .AddSingleton<MassController>()

                // Configuration
                .AddSingleton<IConfigurationFileLocator, UserProfileFileLocator>()
                .AddSingleton<IConfigurationReader, XmlConfigurationReader>()
                .AddSingleton<IConfigurationWriter, XmlConfigurationWriter>()
                .AddSingleton<ConfigurationFactory>()
                .AddSingleton<ConfigurationUpdater>()

                // Shutdown monitor
                .AddSingleton<MonitorForm>()
                .AddSingleton<ShutdownLocker>()

                .AddSingleton(_ =>
                {
                    var currentFile = Process.GetCurrentProcess().MainModule.FileName;
                    var command = $"\"{currentFile}\" --auto-start";

                    return new StartupManager("VBoxHeadlessAutoStart", command);
                })

                .AddSingleton<InstanceLocker>()

                .BuildServiceProvider();

            var locker = serviceProvider.GetService<InstanceLocker>();
            if (!locker.StartLock())
            {
                MonitorForm.BroadcastConfigureMessage();
                return;
            }

            var logger = serviceProvider.GetService<ILogger<TrayApplicationContext>>();
            logger.LogTrace($"TrayApp {Process.GetCurrentProcess().Id} started");

            var appState = serviceProvider.GetService<AppState>();
            appState.OnConfigurationChange += (object sender, ConfigurationChangeEventArgs e) =>
                // Update the log level from the configuration
                LogLevelConfigurationManager.SetLogLevel(e.NewConfiguration.LogLevel);

            // Load the change logger so it attaches its event listeners to the app state
            _ = serviceProvider.GetService<ChangeLogger>();

            try
            {
                // Load the configuration and machines into the store
                appState.UpdateConfiguration();
                appState.UpdateMachines();
            }
            catch (InvalidInstallException e)
            {
                if (!IsAutoStarting())
                {
                    MessageBox.Show(
                        $"Could not continue, {e.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                }
                return;
            }

            if (IsAutoStarting())
            {
                var massController = serviceProvider.GetService<MassController>();
                Task.Run(() => massController.StartAll((_, c) => c?.AutoStart == true));
            }
            else if (!appState.Configuration.ShowTrayIcon)
            {
                var configurationUpdater = serviceProvider.GetService<ConfigurationUpdater>();
                Task.Run(() => configurationUpdater.ShowConfigurationForm());
            }

            // Show the shutdown monitor form so it can listen for shutdown events and block them if needed
            serviceProvider.GetService<MonitorForm>().Show();

            // Run the application
            Application.Run(serviceProvider.GetService<TrayApplicationContext>());

            locker.StopLock();

            logger.LogTrace($"TrayApp {Process.GetCurrentProcess().Id} finished");

            NLog.LogManager.Flush();
            NLog.LogManager.Shutdown();
        }

        private static bool IsAutoStarting()
        {
            return Array.Find(Environment.GetCommandLineArgs(), arg => arg == "--auto-start") != null;
        }
    }
}