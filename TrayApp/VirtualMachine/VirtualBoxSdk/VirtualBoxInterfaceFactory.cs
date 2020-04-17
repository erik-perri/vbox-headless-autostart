using Microsoft.Extensions.Logging;
using System;

namespace TrayApp.VirtualMachine.VirtualBoxSdk
{
    public class VirtualBoxInterfaceFactory
    {
        private readonly ILogger<VirtualBoxInterface> interfaceLogger;

        public VirtualBoxInterfaceFactory(ILogger<VirtualBoxInterface> interfaceLogger)
        {
            this.interfaceLogger = interfaceLogger ?? throw new ArgumentNullException(nameof(interfaceLogger));
        }

        public VirtualBoxInterface Create(string version)
        {
            var parsedVersion = Version.Parse(version);

            return $"{parsedVersion.Major}.{parsedVersion.Minor}" switch
            {
                "6.1" => new VirtualBoxInterface(interfaceLogger),
                _ => throw new InvalidInstallException($"VirtualBox {version} is not supported"),
            };
        }
    }
}