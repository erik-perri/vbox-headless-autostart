using Microsoft.Extensions.DependencyInjection;
using System;
using TrayApp.VirtualMachine.VirtualBoxSdk.Proxy;

namespace TrayApp.VirtualMachine.VirtualBoxSdk
{
    public class VirtualBoxProxyFactory
    {
        private readonly IServiceProvider serviceProvider;

        public VirtualBoxProxyFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IVirtualBoxProxy Create(string version)
        {
            var parsedVersion = Version.Parse(version);

            return $"{parsedVersion.Major}.{parsedVersion.Minor}" switch
            {
                "6.1" => serviceProvider.GetService<Proxy.Version61.VirtualBoxProxy>(),
                _ => throw new InvalidInstallException($"VirtualBox {version} is not supported")
            };
        }
    }
}