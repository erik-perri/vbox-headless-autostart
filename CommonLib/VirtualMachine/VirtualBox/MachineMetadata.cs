using System;

namespace CommonLib.VirtualMachine.VirtualBox
{
    public class MachineMetadata : IMachineMetadata, IEquatable<MachineMetadata>
    {
        public MachineState State { get; }

        public DateTime LastAction { get; }

        public string SessionName { get; }

        public MachineMetadata(MachineState state, DateTime lastAction, string sessionName)
        {
            State = state;
            LastAction = lastAction;
            SessionName = sessionName;
        }

        public MachineMetadata()
            : this(MachineState.Unknown, DateTime.MinValue, null)
        {
        }

        public override string ToString()
        {
            return $"{{ State = {State}, LastAction = \"{LastAction}\", SessionName = \"{SessionName}\" }}";
        }

        public bool Equals(MachineMetadata other)
        {
            if (other != null)
            {
                return other.State == State
                    && other.LastAction == LastAction
                    && other.SessionName == SessionName;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MachineMetadata);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                State.GetHashCode(),
                LastAction.GetHashCode(),
                SessionName.GetHashCode(StringComparison.Ordinal)
            );
        }
    }
}