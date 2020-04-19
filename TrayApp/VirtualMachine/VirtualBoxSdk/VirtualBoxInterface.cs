using System;

namespace TrayApp.VirtualMachine.VirtualBoxSdk
{
    public class VirtualBoxInterface : IMachineController, IMachineLocator
    {
        private readonly VirtualBoxInterfaceFactory factory;
        private readonly string version;

        public VirtualBoxInterface(VirtualBoxInterfaceFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));

            version = VirtualBoxVersionLocator.GetVersion();
        }

        public IMachineMetadata[] ListMachines()
        {
            using var virtualbox = factory.Create(version);

            return virtualbox.ListMachines();
        }

        public bool Start(IMachineMetadata machine, bool headless)
        {
            using var virtualbox = factory.Create(version);

            return virtualbox.Start(machine, headless);
        }

        public bool SaveState(IMachineMetadata machine)
        {
            using var virtualbox = factory.Create(version);

            return virtualbox.SaveState(machine);
        }

        public bool AcpiPowerOff(IMachineMetadata machine, int waitLimitInMilliseconds, Action onWaitAction)
        {
            using var virtualbox = factory.Create(version);

            return virtualbox.AcpiPowerOff(machine, waitLimitInMilliseconds, onWaitAction);
        }

        public bool PowerOff(IMachineMetadata machine)
        {
            using var virtualbox = factory.Create(version);

            return virtualbox.PowerOff(machine);
        }

        public bool Reset(IMachineMetadata machine)
        {
            using var virtualbox = factory.Create(version);

            return virtualbox.Reset(machine);
        }
    }
}