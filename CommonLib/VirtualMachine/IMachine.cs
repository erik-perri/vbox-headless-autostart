namespace CommonLib.VirtualMachine
{
    public interface IMachine
    {
        string Uuid { get; }

        string Name { get; }

        IMachineMetadata Metadata { get; set; }

        public bool IsPoweredOn
        {
            get
            {
                return Metadata.State == MachineState.Running;
            }
        }

        public bool IsPoweredOff
        {
            get
            {
                return Metadata.State == MachineState.Aborted
                    || Metadata.State == MachineState.PoweredOff
                    || Metadata.State == MachineState.Saved;
            }
        }
    }
}