using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace TrayApp.Configuration
{
    public class XmlConfigurationReader : IConfigurationReader
    {
        private readonly ILogger<XmlConfigurationReader> logger;

        public XmlConfigurationReader(ILogger<XmlConfigurationReader> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TrayConfiguration ReadConfiguration()
        {
            var configurationFile = XmlConfigurationFileLocator.LocateConfigurationFile();

            try
            {
                var document = new XmlDocument();
                document.Load(configurationFile);

                return new TrayConfiguration()
                {
                    LogLevel = ReadLogLevel(document),
                    ShowKeepAwakeMenu = ReadShowKeepAwakeMenu(document),
                    Machines = new ReadOnlyCollection<MachineConfiguration>(ReadMachines(document)),
                };
            }
            catch (Exception e) when (
                e is XmlException ||
                e is FileNotFoundException ||
                e is DirectoryNotFoundException
            )
            {
                logger.LogError(e, $"Failed to parse config XML \"{configurationFile}\"");
            }

            return null;
        }

        private LogLevel ReadLogLevel(XmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var logLevelNode = document.DocumentElement.SelectSingleNode("/Configuration/LogLevel");
            if (logLevelNode == null)
            {
                return LogLevel.Information;
            }

            if (!Enum.TryParse(logLevelNode.InnerText.Trim(), out LogLevel logLevel))
            {
                logger.LogError($"Unknown LogLevel \"{logLevelNode.InnerText.Trim()}\" specified");
            }

            return logLevel;
        }

        private MachineConfiguration[] ReadMachines(XmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            List<MachineConfiguration> found = new List<MachineConfiguration>();

            var machinesNodes = document.DocumentElement.SelectSingleNode("/Configuration/Machines");

            if (machinesNodes == null)
            {
                return found.ToArray();
            }

            foreach (XmlNode machineNode in machinesNodes.ChildNodes)
            {
                try
                {
                    var uuid = machineNode.SelectSingleNode("Uuid")?.InnerText;
                    var saveState = machineNode.SelectSingleNode("SaveState")?.InnerText == "true";
                    var autoStart = machineNode.SelectSingleNode("AutoStart")?.InnerText == "true";

                    if (uuid == null || uuid.Trim().Length < 1)
                    {
                        logger.LogWarning("Skipping machine specified with no UUID");
                        continue;
                    }

                    found.Add(new MachineConfiguration()
                    {
                        Uuid = uuid,
                        SaveState = saveState,
                        AutoStart = autoStart,
                    });
                }
                catch (XPathException e)
                {
                    logger.LogError(e, "Failed to parse machine config");
                }
            }

            return found.ToArray();
        }
    }
}