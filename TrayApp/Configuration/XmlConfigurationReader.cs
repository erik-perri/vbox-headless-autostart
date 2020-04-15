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
                var document = new XmlDocument();
                document.Load(configurationFile);

                return new AppConfiguration(
                    ReadLogLevel(document),
                    ReadShowKeepAwakeMenu(document),
                    new ReadOnlyCollection<MachineConfiguration>(ReadMachines(document))
                );
            }
            catch (Exception e) when (
                e is XmlException ||
                e is FileNotFoundException ||
                e is DirectoryNotFoundException
            )
            {
                logger.LogError(e, $"Failed to parse XML file {new { File = configurationFile, Error = e.Message }}");
            }

            return null;
        }

        private LogLevel ReadLogLevel(XmlDocument document)
        {
            var level = ReadString(document, "/Configuration/LogLevel");

            if (!Enum.TryParse(level, out LogLevel logLevel))
            {
                logger.LogError($"Unknown LogLevel {new { Level = level }}");
            }

            return logLevel;
        }

        private bool ReadShowKeepAwakeMenu(XmlDocument document)
        {
            var show = ReadString(document, "/Configuration/ShowKeepAwakeMenu");

            return show.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private static string ReadString(XmlDocument document, string path)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var node = document.DocumentElement.SelectSingleNode(path);
            if (node == null)
            {
                return null;
            }

            return node.InnerText.Trim();
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
                    logger.LogError(e, $"Failed to parse machine config {new { Error = e.Message }}");
                }
            }

            return found.ToArray();
        }
    }
}