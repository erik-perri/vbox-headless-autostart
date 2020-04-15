using System;

namespace TrayApp.VirtualMachine
{
    public class MachineMetadata : IMachineMetadata, IEquatable<MachineMetadata>
    {
        public string Uuid { get; }

        public string Name { get; }

        public MachineState State { get; }

        public DateTime LastAction { get; }

        public string SessionName { get; }

        public MachineMetadata(string uuid, string name, MachineState state, DateTime lastAction, string sessionName)
        {
            Uuid = uuid;
            Name = name;
            State = state;
            LastAction = lastAction;
            SessionName = sessionName;
        }

        public MachineMetadata(string uuid, string name)
            : this(uuid, name, MachineState.Unknown, DateTime.MinValue, null)
        {
        }

        public override string ToString()
        {
            return $"{new { Uuid, Name, State, LastAction, SessionName }}";
        }

        public bool Equals(MachineMetadata other)
        {
            if (other != null)
            {
                return other.Uuid == Uuid
                    && other.Name == Name
                    && other.State == State
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
                Uuid.GetHashCode(StringComparison.Ordinal),
                Name.GetHashCode(StringComparison.Ordinal),
                State.GetHashCode(),
                LastAction.GetHashCode(),
                SessionName.GetHashCode(StringComparison.Ordinal)
            );
        }
    }
}