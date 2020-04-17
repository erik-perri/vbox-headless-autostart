using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace TrayApp.Configuration
{
    public class XmlConfigurationReader : IConfigurationReader
    {
        private readonly ILogger<XmlConfigurationReader> logger;
        private readonly IConfigurationFileLocator fileLocator;

        public XmlConfigurationReader(ILogger<XmlConfigurationReader> logger, IConfigurationFileLocator fileLocator)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.fileLocator = fileLocator ?? throw new ArgumentNullException(nameof(fileLocator));
        }

        public AppConfiguration ReadConfiguration()
        {
            var configurationFile = fileLocator.LocateFile();

            try
            {
                using var reader = XmlReader.Create(configurationFile);
                var serializer = new DataContractSerializer(typeof(AppConfiguration));

                return (AppConfiguration)serializer.ReadObject(reader);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (Exception e) when (e is XmlException || e is SerializationException)
            {
                logger.LogError(e, "Failed to read configuration file");
                return null;
            }
        }
    }
}