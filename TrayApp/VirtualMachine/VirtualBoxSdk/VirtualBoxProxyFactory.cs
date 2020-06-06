using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using TrayApp.VirtualMachine.VirtualBoxSdk.Proxy;

namespace TrayApp.VirtualMachine.VirtualBoxSdk
{
    public class VirtualBoxProxyFactory
    {
        private readonly ILogger<VirtualBoxProxyFactory> logger;
        private readonly IServiceProvider serviceProvider;

        public VirtualBoxProxyFactory(ILogger<VirtualBoxProxyFactory> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public IVirtualBoxProxy Create(string version)
        {
            var parsedVersion = Version.Parse(version);

            try
            {
                return $"{parsedVersion.Major}.{parsedVersion.Minor}" switch
                {
                    "6.0" => serviceProvider.GetService<Proxy.Version60.VirtualBoxProxy>(),
                    "6.1" => serviceProvider.GetService<Proxy.Version61.VirtualBoxProxy>(),
                    _ => throw new InvalidInstallException($"VirtualBox {version} is not supported")
                };
            }
            catch (COMException e)
            {
                logger.LogError(e, "Failed to create VirtualBox COM instance");
                return null;
            }
        }
    }
}