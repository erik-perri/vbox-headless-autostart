using CommonLib.Helpers;

namespace CommonLib.VirtualMachine
{
    public interface IMachineMetadata : IUuidContainer
    {
        string Name { get; }

        public MachineState State { get; }

        public bool IsPoweredOn
        {
            get
            {
                return State == MachineState.Running;
            }
        }

        public bool IsPoweredOff
        {
            get
            {
                return State == MachineState.Aborted
                    || State == MachineState.PoweredOff
                    || State == MachineState.Saved;
            }
        }
    }
}