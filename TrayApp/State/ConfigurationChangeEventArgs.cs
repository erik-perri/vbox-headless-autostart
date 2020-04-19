using System;
using TrayApp.Configuration;

namespace TrayApp.State
{
    public class ConfigurationChangeEventArgs : EventArgs
    {
        public AppConfiguration PreviousConfiguration { get; }

        public AppConfiguration NewConfiguration { get; }

        public ConfigurationChangeEventArgs(AppConfiguration previousConfiguration, AppConfiguration newConfiguration)
        {
            PreviousConfiguration = previousConfiguration;
            NewConfiguration = newConfiguration;
        }
    }
}