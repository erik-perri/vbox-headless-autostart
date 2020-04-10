using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TrayApp.Configuration
{
    public class TrayConfiguration : IEquatable<TrayConfiguration>
    {
        public LogLevel LogLevel { get; internal set; }

        public bool ShowKeepAwakeMenu { get; internal set; }

        public ReadOnlyCollection<MachineConfiguration> Machines { get; internal set; }

        public bool Equals(TrayConfiguration other)
        {
            return other != null
                && LogLevel == other.LogLevel
                && Machines.OrderBy(m => m.Uuid).SequenceEqual(other.Machines.OrderBy(m => m.Uuid));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TrayConfiguration);
        }

        public override int GetHashCode()
        {
            var hashCode = LogLevel.GetHashCode();

            foreach (var machine in Machines)
            {
                hashCode = HashCode.Combine(hashCode, machine.GetHashCode());
            }

            return hashCode;
        }
    }
}