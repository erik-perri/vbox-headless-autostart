using System;
using System.IO;

namespace TrayApp.Configuration
{
    public class UserProfileFileLocator : IConfigurationFileLocator
    {
        private readonly string profilePath;

        public UserProfileFileLocator()
        {
            profilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        public UserProfileFileLocator(string profilePath)
        {
            if (!Directory.Exists(profilePath))
            {
                throw new ArgumentException($"User profile \"{profilePath}\" does not exist", nameof(profilePath));
            }

            this.profilePath = profilePath;
        }

        public string LocateFile()
        {
            return $"{profilePath}\\.VirtualBox\\VBoxHeadlessAutoStart.xml";
        }
    }
}