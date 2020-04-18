using System;
using TrayApp.VirtualMachine.VirtualBoxSdk;

namespace TrayApp.VirtualMachine
{
    public class VirtualBoxController : IMachineController, IMachineLocator
    {
        private readonly VirtualBoxInterfaceFactory factory;

        public VirtualBoxController(VirtualBoxInterfaceFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IMachineMetadata[] ListMachines()
        {
            using var virtualbox = factory.Create(VirtualBoxVersionLocator.GetVersion());

            return virtualbox.ListMachines();
        }

        public bool Start(IMachineMetadata machine, bool headless)
        {
            using var virtualbox = factory.Create(VirtualBoxVersionLocator.GetVersion());

            return virtualbox.Start(machine, headless);
        }

        public bool SaveState(IMachineMetadata machine)
        {
            using var virtualbox = factory.Create(VirtualBoxVersionLocator.GetVersion());

            return virtualbox.SaveState(machine);
        }

        public bool AcpiPowerOff(IMachineMetadata machine, int waitLimitInMilliseconds, Action onWaitAction)
        {
            using var virtualbox = factory.Create(VirtualBoxVersionLocator.GetVersion());

            return virtualbox.AcpiPowerOff(machine, waitLimitInMilliseconds, onWaitAction);
        }

        public bool PowerOff(IMachineMetadata machine)
        {
            using var virtualbox = factory.Create(VirtualBoxVersionLocator.GetVersion());

            return virtualbox.PowerOff(machine);
        }

        public bool Reset(IMachineMetadata machine)
        {
            using var virtualbox = factory.Create(VirtualBoxVersionLocator.GetVersion());

            return virtualbox.Reset(machine);
        }
    }
}