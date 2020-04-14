using Microsoft.Extensions.Logging;
using System;
using System.Xml.Serialization;

namespace CommonLib.Configuration
{
    [XmlType(TypeName = "Configuration")]
    [Serializable]
    public class AppConfigurationXmlMapping
    {
        public LogLevel LogLevel { get; set; }

        public bool ShowKeepAwakeMenu { get; set; }

        [XmlArray("Machines")]
        [XmlArrayItem(ElementName = "Machine")]
#pragma warning disable CA1819 // Properties should not return arrays
        public MachineConfiguration[] Machines { get; set; }

#pragma warning restore CA1819 // Properties should not return arrays
    }
}