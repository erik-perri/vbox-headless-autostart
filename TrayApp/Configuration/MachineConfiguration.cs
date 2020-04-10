using System;

namespace TrayApp.Configuration
{
    public class MachineConfiguration : IEquatable<MachineConfiguration>
    {
        public string Uuid { get; set; }

        public bool SaveState { get; set; }

        public bool AutoStart { get; set; }

        public bool Equals(MachineConfiguration other)
        {
            return other != null
                && Uuid == other.Uuid
                && SaveState == other.SaveState
                && AutoStart == other.AutoStart;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MachineConfiguration);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Uuid.GetHashCode(StringComparison.Ordinal), SaveState, AutoStart);
        }

        public static MachineConfiguration GetDefaultConfiguration(string uuid)
        {
            return new MachineConfiguration()
            {
                Uuid = uuid,
                AutoStart = false,
                SaveState = true,
            };
        }
    }
}