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
        public bool ShowMenu { get; private set; }

        [DataMember]
        public bool AutoStart { get; private set; }

        [DataMember]
        public bool AutoStop { get; private set; }

        [DataMember]
        public bool SaveState { get; private set; }

        public MachineConfiguration(string uuid, bool showMenu, bool autoStart, bool autoStop, bool saveState)
        {
            Uuid = uuid;
            ShowMenu = showMenu;
            AutoStart = autoStart;
            AutoStop = autoStop;
            SaveState = saveState;
        }

        public bool Equals(MachineConfiguration other)
        {
            return other != null
                && Uuid == other.Uuid
                && ShowMenu == other.ShowMenu
                && AutoStart == other.AutoStart
                && AutoStop == other.AutoStop
                && SaveState == other.SaveState;
        }

        public override string ToString()
        {
            return $"{new { Uuid, ShowMenu, AutoStart, AutoStop, SaveState }}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MachineConfiguration);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Uuid.GetHashCode(StringComparison.Ordinal),
                ShowMenu,
                AutoStart,
                AutoStop,
                SaveState
            );
        }
    }
}