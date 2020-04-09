using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Windows.Forms;

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
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog("NLog.config.xml");
                })
                .AddSingleton<TrayAppContext>()
                .BuildServiceProvider();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var context = serviceProvider.GetService<TrayAppContext>();

            Application.Run(context);
        }
    }
}