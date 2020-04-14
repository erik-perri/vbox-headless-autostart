using CommonLib.Configuration;
using System;

namespace TrayApp.Configuration
{
    public class UserProfileFileLocator : IConfigurationFileLocator
    {
        public string LocateFile()
        {
            var profilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return $"{profilePath}\\.VirtualBox\\VBoxHeadlessAutoStart.xml";
        }
    }
}