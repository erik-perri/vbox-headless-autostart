using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Linq;
using System.Windows.Forms;
using TrayApp.Menu;
using TrayApp.Menu.Handler;

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
                    provider.GetServices<IMenuHandler>().OrderBy(m => m.GetSortOrder()).ToArray()
                ))
                    .AddSingleton<IMenuHandler, ExitMenuHandler>()
                    .AddSingleton<IMenuHandler, ConfigureMenuHandler>()

                .BuildServiceProvider();

            var context = serviceProvider.GetService<TrayApplicationContext>();

            Application.Run(context);
        }
    }
}