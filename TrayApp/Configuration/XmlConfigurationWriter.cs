using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Xml.Serialization;

namespace TrayApp.Configuration
{
    public class XmlConfigurationWriter : IConfigurationWriter
    {
        private readonly ILogger<XmlConfigurationWriter> logger;

        public XmlConfigurationWriter(ILogger<XmlConfigurationWriter> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool WriteConfiguration(TrayConfiguration configuration)
        {
            var configurationFile = XmlConfigurationFileLocator.LocateConfigurationFile();

            try
            {
                var writer = new XmlSerializer(typeof(TrayConfiguration));
                var stream = File.Create(configurationFile);
                writer.Serialize(stream, configuration);
                stream.Close();
                return true;
            }
            catch (Exception e) when (e is InvalidOperationException
                                    || e is DirectoryNotFoundException)
            {
                logger.LogError(e, $"Failed to write configuration to \"{configurationFile}\"");
            }

            return false;
        }
    }
}