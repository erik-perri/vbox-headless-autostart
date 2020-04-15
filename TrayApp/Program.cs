using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Linq;
using System.Windows.Forms;
using TrayApp.AutoControl;
using TrayApp.Configuration;
using TrayApp.Forms;
using TrayApp.KeepAwake;
using TrayApp.Logging;
using TrayApp.Menu;
using TrayApp.Menu.Handler;
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
                        .AddSingleton<IMachineController, VirtualBoxController>()
                    .AddSingleton<IMenuHandler, KeepAwakeMenuHandler>()
                        .AddSingleton<KeepAwakeTask>()
                    .AddSingleton<IMenuHandler, MassControlMenuHandler>()

                // Machine locator
                .AddSingleton<IMachineLocator, VirtualBoxController>()

                // Configuration
                .AddSingleton<IConfigurationFileLocator, UserProfileFileLocator>()
                .AddSingleton<IConfigurationReader, XmlConfigurationReader>()
                .AddSingleton<IConfigurationWriter, XmlConfigurationWriter>()

                .AddSingleton<AutoController>()

                .AddSingleton<AppState>()
                .AddSingleton<MachineStateUpdater>()

                .AddSingleton<VirtualBoxController>()
                    .AddSingleton(new VirtualBox.VirtualBox())

                .AddSingleton<ShutdownMonitorForm>()
                .AddSingleton<ShutdownMonitor>()
                    .AddSingleton(provider => new ShutdownBlock(
                        provider.GetService<ILogger<ShutdownBlock>>(),
                        provider.GetService<TrayContextMenuStrip>()
                    ))

                .BuildServiceProvider();

            // Load the configuration and machines into the store
            var appState = serviceProvider.GetService<AppState>();
            appState.UpdateConfiguration();
            appState.UpdateMachines();

            // Set the log level from the configuration
            LogLevelConfigurationManager.SetLogLevel(appState.Configuration.LogLevel);

            if (IsAutoStarting())
            {
                serviceProvider.GetService<AutoController>().StartMachines();
            }

            // Start the machine state monitor
            serviceProvider.GetService<MachineStateUpdater>().StartMonitor();

            serviceProvider.GetService<ShutdownMonitorForm>().Show();

            // Run the application
            Application.Run(serviceProvider.GetService<TrayApplicationContext>());
        }

        private static bool IsAutoStarting()
        {
            return Array.Find(Environment.GetCommandLineArgs(), arg => arg == "--auto-start") != null;
        }
    }
}