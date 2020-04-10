using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Linq;
using System.Windows.Forms;
using TrayApp.Configuration;
using TrayApp.Logging;
using TrayApp.Menu;
using TrayApp.Menu.Handler;
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
                    .AddSingleton<TrayIconManager>()
                    .AddSingleton<TrayContextMenuStrip>()

                // Menu
                .AddSingleton(provider => new TrayContextMenuStrip(
                    provider.GetService<ILogger<TrayContextMenuStrip>>(),
                    provider.GetServices<IMenuHandler>().OrderBy(m => m.GetSortOrder()).ToArray()
                ))
                    .AddSingleton<IMenuHandler, ExitMenuHandler>()
                    .AddSingleton<IMenuHandler, ConfigureMenuHandler>()
                    .AddSingleton<IMenuHandler, MachineControlMenuHandler>()

                // Machine locator
                .AddSingleton<MachineStore>()
                .AddSingleton<MachineStoreUpdater>()
                .AddSingleton<ILocatorService, VirtualMachine.VirtualBox.LocatorService>()
                    .AddSingleton<VirtualMachine.VirtualBox.MetadataReader>()
                    .AddSingleton<MonitoredMachineFilter>()

                // Configuration
                .AddSingleton<ConfigurationStore>()
                .AddSingleton<IConfigurationReader, XmlConfigurationReader>()
                .AddSingleton<IConfigurationWriter, XmlConfigurationWriter>()

                // Controller
                .AddSingleton<IMachineController, VirtualMachine.VirtualBox.MachineController>()

                .BuildServiceProvider();

            // Load the configuration into the store
            var configurationStore = serviceProvider.GetService<ConfigurationStore>();
            configurationStore.UpdateConfiguration();

            // Set the log level from the configuration
            LogLevelConfigurationManager.SetLogLevel(configurationStore.GetConfiguration().LogLevel);

            // Start the machine state monitor
            var machineStoreUpdater = serviceProvider.GetService<MachineStoreUpdater>();
            machineStoreUpdater.StartMonitor();

            // Run the application
            var context = serviceProvider.GetService<TrayApplicationContext>();

            Application.Run(context);
        }
    }
}