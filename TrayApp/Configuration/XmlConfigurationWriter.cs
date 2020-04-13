using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace TrayApp.Configuration
{
    public class XmlConfigurationWriter : IConfigurationWriter
    {
        public void WriteConfiguration(AppConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var configurationFile = XmlConfigurationFileLocator.LocateConfigurationFile();

            var configuratingMapping = new AppConfigurationXmlMapping()
            {
                LogLevel = configuration.LogLevel,
                ShowKeepAwakeMenu = configuration.ShowKeepAwakeMenu,
                Machines = configuration.Machines.ToArray(),
            };

            var writer = new XmlSerializer(typeof(AppConfigurationXmlMapping));
            var stream = File.Create(configurationFile);
            writer.Serialize(stream, configuratingMapping);
            stream.Close();
        }
    }
}