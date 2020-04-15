using CommonLib.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace ServiceApp.Configuration
{
    public class XmlConfigurationReaderFactory
    {
        private readonly ILogger<XmlConfigurationReader> readerLogger;

        public XmlConfigurationReaderFactory(ILogger<XmlConfigurationReader> readerLogger)
        {
            this.readerLogger = readerLogger ?? throw new ArgumentNullException(nameof(readerLogger));
        }

        public XmlConfigurationReader CreateReader(string profilePath)
        {
            return new XmlConfigurationReader(readerLogger, new UserProfileFileLocator(profilePath));
        }
    }
}