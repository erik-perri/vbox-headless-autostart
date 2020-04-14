using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CommonLib.Configuration
{
    public class AppConfiguration : IEquatable<AppConfiguration>
    {
        public LogLevel LogLevel { get; }

        public bool ShowKeepAwakeMenu { get; }

        public ReadOnlyCollection<MachineConfiguration> Machines { get; }

        public AppConfiguration(LogLevel logLevel, bool showKeepAwakeMenu, ReadOnlyCollection<MachineConfiguration> machines)
        {
            LogLevel = logLevel;
            ShowKeepAwakeMenu = showKeepAwakeMenu;
            Machines = machines;
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
            var hashCode = HashCode.Combine(LogLevel.GetHashCode(), ShowKeepAwakeMenu);

            foreach (var machine in Machines)
            {
                hashCode = HashCode.Combine(hashCode, machine.GetHashCode());
            }

            return hashCode;
        }
    }
}