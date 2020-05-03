using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace TrayApp.Configuration
{
    [DataContract(Name = "AppConfiguration")]
    public class AppConfiguration : IEquatable<AppConfiguration>
    {
        [DataMember]
        public LogLevel LogLevel { get; private set; }

        [DataMember]
        public bool StartWithWindows { get; private set; }

        [DataMember]
        public bool ShowTrayIcon { get; set; }

        [DataMember]
        public bool ShowKeepAwakeMenu { get; private set; }

        [DataMember]
        public ReadOnlyCollection<MachineConfiguration> Machines { get; private set; }

        public AppConfiguration(
            LogLevel logLevel,
            bool startWithWindows,
            bool showTrayIcon,
            bool showKeepAwakeMenu,
            ReadOnlyCollection<MachineConfiguration> machines
        )
        {
            LogLevel = logLevel;
            StartWithWindows = startWithWindows;
            ShowTrayIcon = showTrayIcon;
            ShowKeepAwakeMenu = showKeepAwakeMenu;
            Machines = machines;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext _)
        {
            ShowTrayIcon = true;
        }

        public override string ToString()
        {
            return $"{new { LogLevel, StartWithWindows, ShowTrayIcon, ShowKeepAwakeMenu, Machines = Machines.Count }}";
        }

        public bool Equals(AppConfiguration other)
        {
            return other != null
                && LogLevel == other.LogLevel
                && StartWithWindows == other.StartWithWindows
                && ShowTrayIcon == other.ShowTrayIcon
                && ShowKeepAwakeMenu == other.ShowKeepAwakeMenu
                && Machines.OrderBy(m => m.Uuid).SequenceEqual(other.Machines.OrderBy(m => m.Uuid));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AppConfiguration);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(LogLevel);
            hashCode.Add(StartWithWindows);
            hashCode.Add(ShowTrayIcon);
            hashCode.Add(ShowKeepAwakeMenu);

            foreach (var machine in Machines)
            {
                hashCode.Add(machine);
            }

            return hashCode.ToHashCode();
        }
    }
}