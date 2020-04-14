using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CommonLib.Configuration
{
    public class XmlConfigurationWriter : IConfigurationWriter
    {
        private readonly IConfigurationFileLocator fileLocator;

        public XmlConfigurationWriter(IConfigurationFileLocator fileLocator)
        {
            this.fileLocator = fileLocator ?? throw new ArgumentNullException(nameof(fileLocator));
        }

        public void WriteConfiguration(AppConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var configurationFile = fileLocator.LocateFile();

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