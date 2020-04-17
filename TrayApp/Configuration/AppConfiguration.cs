using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using TrayApp.Logging;

namespace TrayApp.Configuration
{
    [DataContract(Name = "AppConfiguration")]
    public class AppConfiguration : IEquatable<AppConfiguration>
    {
        [DataMember]
        public LogLevel LogLevel { get; private set; }

        [DataMember]
        public bool ShowKeepAwakeMenu { get; private set; }

        [DataMember]
        public ReadOnlyCollection<MachineConfiguration> Machines { get; private set; }

        public AppConfiguration(LogLevel logLevel, bool showKeepAwakeMenu, ReadOnlyCollection<MachineConfiguration> machines)
        {
            LogLevel = logLevel;
            ShowKeepAwakeMenu = showKeepAwakeMenu;
            Machines = machines;
        }

        public static AppConfiguration GetDefaultConfiguration()
        {
            return new AppConfiguration(
                LogLevelConfigurationManager.DefaultLevel,
                false,
                new ReadOnlyCollection<MachineConfiguration>(Array.Empty<MachineConfiguration>())
            );
        }

        public override string ToString()
        {
            return $"{new { LogLevel, ShowKeepAwakeMenu, MachineCount = Machines.Count }}";
        }

        public bool Equals(AppConfiguration other)
        {
            return other != null
                && LogLevel == other.LogLevel
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
            hashCode.Add(ShowKeepAwakeMenu);

            foreach (var machine in Machines)
            {
                hashCode.Add(machine);
            }

            return hashCode.ToHashCode();
        }
    }
}