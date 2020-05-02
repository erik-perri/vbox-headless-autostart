using TrayApp.Helpers;

namespace TrayApp.VirtualMachine
{
    public interface IMachineMetadata : IUuidContainer
    {
        string Name { get; }

        public VirtualMachineState State { get; }

        public bool IsPoweredOn
        {
            get
            {
                return State == VirtualMachineState.Running;
            }
        }

        public bool IsPoweredOff
        {
            get
            {
                return State == VirtualMachineState.Aborted
                    || State == VirtualMachineState.PoweredOff
                    || State == VirtualMachineState.Saved;
            }
        }
    }
}