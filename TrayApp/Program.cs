using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
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

                // Application context
                .AddSingleton<TrayApplicationContext>()
                    .AddSingleton<NotifyIconManager>()
                    .AddSingleton<TrayContextMenuStrip>()

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
                .AddSingleton<IMachineController, VirtualBoxController>()
                .AddSingleton<IMachineLocator, VirtualBoxController>()
                .AddSingleton<MachineStateUpdater>()
                .AddSingleton<MassController>()
                .AddSingleton(new VirtualBox.VirtualBox())

                // Configuration
                .AddSingleton<IConfigurationFileLocator, UserProfileFileLocator>()
                .AddSingleton<IConfigurationReader, XmlConfigurationReader>()
                .AddSingleton<IConfigurationWriter, XmlConfigurationWriter>()
                .AddSingleton<ConfigurationFactory>()

                // Shutdown monitor
                .AddSingleton<ShutdownMonitorForm>()
                .AddSingleton<ShutdownLocker>()

                .AddSingleton(_ =>
                {
                    var currentFile = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    var command = $"\"{currentFile}\" --auto-start";

                    return new StartupManager("VBoxHeadlessAutoStart", command);
                })

                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<TrayApplicationContext>>();
            logger.LogTrace("TrayApp started");

            var appState = serviceProvider.GetService<AppState>();
            appState.OnConfigurationChange += (object sender, EventArgs e) =>
                // Set the log level from the configuration
                LogLevelConfigurationManager.SetLogLevel(appState.Configuration.LogLevel);

            // Load the configuration and machines into the store
            appState.UpdateConfiguration();
            appState.UpdateMachines();

            if (IsAutoStarting())
            {
                Task.Factory.StartNew(
                    () => serviceProvider.GetService<MassController>().StartAll(),
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskScheduler.Default
                );
            }

            // Start the machine state monitor to update the machine state occasionally
            serviceProvider.GetService<MachineStateUpdater>().StartMonitor();

            // Show the shutdown monitor form so it can listen for shutdown events and block them if needed
            serviceProvider.GetService<ShutdownMonitorForm>().Show();

            // Run the application
            Application.Run(serviceProvider.GetService<TrayApplicationContext>());

            logger.LogTrace("TrayApp finished");

            NLog.LogManager.Flush();
            NLog.LogManager.Shutdown();
        }

        private static bool IsAutoStarting()
        {
            return Array.Find(Environment.GetCommandLineArgs(), arg => arg == "--auto-start") != null;
        }
    }
}