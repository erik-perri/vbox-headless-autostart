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
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog("NLog.config.xml");
                })
                // Application context
                .AddSingleton<TrayApplicationContext>()

                    // Dependencies
                    .AddSingleton<TrayIconManager>()
                    .AddSingleton<TrayContextMenuStrip>()

                // Menu handlers
                .AddSingleton<IMenuHandler, ExitMenuHandler>()
                .AddSingleton(provider => new TrayContextMenuStrip(provider.GetServices<IMenuHandler>().OrderBy(m => m.GetSortOrder()).ToArray()))

                .BuildServiceProvider();

            var context = serviceProvider.GetService<TrayApplicationContext>();

            Application.Run(context);
        }
    }
}