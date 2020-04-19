using Microsoft.Extensions.Logging;
using System;

namespace TrayApp.VirtualMachine.VirtualBoxSdk
{
    public class VirtualBoxInterfaceFactory
    {
        private readonly ILogger<VirtualBoxInterface616> interfaceLogger;

        public VirtualBoxInterfaceFactory(ILogger<VirtualBoxInterface616> interfaceLogger)
        {
            this.interfaceLogger = interfaceLogger ?? throw new ArgumentNullException(nameof(interfaceLogger));
        }

        public VirtualBoxInterface616 Create(string version)
        {
            var parsedVersion = Version.Parse(version);

            return $"{parsedVersion.Major}.{parsedVersion.Minor}" switch
            {
                "6.1" => new VirtualBoxInterface616(interfaceLogger),
                _ => throw new InvalidInstallException($"VirtualBox {version} is not supported"),
            };
        }
    }
}