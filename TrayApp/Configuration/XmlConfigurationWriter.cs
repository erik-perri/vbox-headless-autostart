using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace TrayApp.Configuration
{
    public class XmlConfigurationWriter : IConfigurationWriter
    {
        public void WriteConfiguration(TrayConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var configurationFile = XmlConfigurationFileLocator.LocateConfigurationFile();

            var configuratingMapping = new TrayConfigurationXmlMapping()
            {
                LogLevel = configuration.LogLevel,
                ShowKeepAwakeMenu = configuration.ShowKeepAwakeMenu,
                Machines = configuration.Machines.ToArray(),
            };

            var writer = new XmlSerializer(typeof(TrayConfigurationXmlMapping));
            var stream = File.Create(configurationFile);
            writer.Serialize(stream, configuratingMapping);
            stream.Close();
        }
    }
}