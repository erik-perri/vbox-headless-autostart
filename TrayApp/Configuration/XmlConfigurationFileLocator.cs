using System;

namespace TrayApp.Configuration
{
    public static class XmlConfigurationFileLocator
    {
        public static string LocateConfigurationFile()
        {
            var profilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return FormattableString.Invariant($"{profilePath}\\.VirtualBox\\VBoxHeadlessAutoStart.xml");
        }
    }
}