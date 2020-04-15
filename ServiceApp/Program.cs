using CommonLib.Configuration;
using CommonLib.Processes;
using CommonLib.VirtualMachine;
using CommonLib.VirtualMachine.VirtualBox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ServiceApp.Configuration;
using ServiceApp.Processes;
using System;
using System.ServiceProcess;

namespace ServiceApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            using var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog("NLog.config.xml");
                })

                .AddSingleton<VBoxHeadlessAutoStart>()
                    .AddSingleton<UserProfileLocator>()

                    .AddSingleton<XmlConfigurationReaderFactory>()
                        .AddSingleton<IConfigurationReader, XmlConfigurationReader>()

                .AddSingleton<IMachineController, VBoxManageOutputFactory>()
                .AddSingleton<IMachineLocator, VBoxManageOutputFactory>()
                    .AddSingleton<IProcessOutputFactory, ImpersonatedProcessOutputFactory>()

                .BuildServiceProvider();

            try
            {
                ServiceBase.Run(new ServiceBase[]
                {
                    serviceProvider.GetService<VBoxHeadlessAutoStart>()
                });
            }
            // We need to catch InvalidOperationException errors to properly log DependencyInjection errors. Unhandled
            // exceptions write a system event log but it does not contain a useful failure reason.
            catch (Exception e) when (e is InvalidOperationException)
            {
                serviceProvider.GetService<ILogger<VBoxHeadlessAutoStart>>().LogCritical(e, "Caught exception");
            }

            NLog.LogManager.Shutdown();
        }
    }
}