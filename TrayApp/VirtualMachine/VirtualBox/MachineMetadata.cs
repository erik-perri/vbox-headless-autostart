using System;

namespace TrayApp.VirtualMachine.VirtualBox
{
    public class MachineMetadata : IMachineMetadata, IEquatable<MachineMetadata>
    {
        public MachineState State { get; internal set; } = MachineState.Unknown;

        public DateTime LastAction { get; internal set; } = DateTime.MinValue;

        public string SessionName { get; internal set; } = null;

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