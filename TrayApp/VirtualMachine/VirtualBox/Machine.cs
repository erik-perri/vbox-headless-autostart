using System;

namespace TrayApp.VirtualMachine.VirtualBox
{
    public class Machine : IMachine, IEquatable<Machine>
    {
        public string Uuid { get; }

        public string Name { get; }

        public IMachineMetadata Metadata { get; set; }

        public Machine(string uuid, string name)
        {
            Uuid = uuid;
            Name = name;
        }

        public Machine(string uuid, string name, IMachineMetadata metadata)
        {
            Uuid = uuid;
            Name = name;
            Metadata = metadata;
        }

        public bool Equals(Machine other)
        {
            if (other != null)
            {
                return other.Uuid.Equals(Uuid, StringComparison.Ordinal)
                    && other.Name.Equals(Name, StringComparison.Ordinal)
                    && (other.Metadata == null ? other.Metadata == Metadata : other.Metadata.Equals(Metadata));
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Machine);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Uuid.GetHashCode(StringComparison.Ordinal),
                Name.GetHashCode(StringComparison.Ordinal),
                Metadata.GetHashCode()
            );
        }
    }
}