using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace TrayApp.Configuration
{
    [XmlType(TypeName = "Configuration")]
    [Serializable]
    public class TrayConfiguration
    {
        public LogLevel LogLevel { get; }

        [XmlArray("Machines")]
        public ReadOnlyCollection<MachineConfiguration> Machines { get; }

        public TrayConfiguration(LogLevel logLevel, MachineConfiguration[] machines)
        {
            LogLevel = logLevel;
            Machines = new ReadOnlyCollection<MachineConfiguration>(machines.ToArray());
        }

        public TrayConfiguration(LogLevel logLevel)
        {
            LogLevel = logLevel;
            Machines = new ReadOnlyCollection<MachineConfiguration>(Array.Empty<MachineConfiguration>());
        }
    }
}