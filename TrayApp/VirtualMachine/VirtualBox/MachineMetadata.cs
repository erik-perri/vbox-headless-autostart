using System;

namespace TrayApp.VirtualMachine.VirtualBox
{
    public class MachineMetadata : IMachineMetadata, IEquatable<MachineMetadata>
    {
        public MachineState State { get; }

        public DateTime LastAction { get; }

        public MachineMetadata(MachineState state, DateTime lastAction)
        {
            State = state;
            LastAction = lastAction;
        }

        public override string ToString()
        {
            return $"{{ State = {State}, LastAction = {LastAction} }}";
        }

        public bool Equals(MachineMetadata other)
        {
            if (other != null)
            {
                return other.State == State
                    && other.LastAction == LastAction;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MachineMetadata);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(State.GetHashCode(), LastAction.GetHashCode());
        }
    }
}