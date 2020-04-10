using Microsoft.Extensions.Logging;
using System;
using System.Xml.Serialization;

namespace TrayApp.Configuration
{
    [XmlType(TypeName = "Configuration")]
    [Serializable]
    public class TrayConfigurationXml
    {
        public LogLevel LogLevel { get; set; }

        public bool ShowKeepAwakeMenu { get; set; }

        [XmlArray("Machines")]
#pragma warning disable CA1819 // Properties should not return arrays
        public MachineConfiguration[] Machines { get; set; }

#pragma warning restore CA1819 // Properties should not return arrays
    }
}