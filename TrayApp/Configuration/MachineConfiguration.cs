using System;
using System.Runtime.Serialization;
using TrayApp.Helpers;

namespace TrayApp.Configuration
{
    [DataContract]
    public class MachineConfiguration : IEquatable<MachineConfiguration>, IUuidContainer
    {
        [DataMember]
        public string Uuid { get; private set; }

        [DataMember]
        public bool SaveState { get; private set; }

        [DataMember]
        public bool AutoStart { get; private set; }

        public MachineConfiguration(string uuid, bool saveState, bool autoStart)
        {
            Uuid = uuid;
            SaveState = saveState;
            AutoStart = autoStart;
        }

        public bool Equals(MachineConfiguration other)
        {
            return other != null
                && Uuid == other.Uuid
                && SaveState == other.SaveState
                && AutoStart == other.AutoStart;
        }

        public override string ToString()
        {
            return $"{new { Uuid, SaveState, AutoStart }}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MachineConfiguration);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Uuid.GetHashCode(StringComparison.Ordinal), SaveState, AutoStart);
        }
    }
}