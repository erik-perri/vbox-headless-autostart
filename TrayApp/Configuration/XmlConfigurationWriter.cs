using System;
using System.Runtime.Serialization;
using System.Xml;

namespace TrayApp.Configuration
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

            using var writer = XmlWriter.Create(configurationFile, new XmlWriterSettings { Indent = true });
            var serializer = new DataContractSerializer(typeof(AppConfiguration));

            serializer.WriteObject(writer, configuration);
        }
    }
}