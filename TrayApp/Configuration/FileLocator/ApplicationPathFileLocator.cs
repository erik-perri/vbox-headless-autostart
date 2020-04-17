using System.IO;
using System.Reflection;

namespace TrayApp.Configuration.FileLocator
{
    public class ApplicationPathFileLocator : IConfigurationFileLocator
    {
        private readonly string profilePath;

        public ApplicationPathFileLocator()
        {
            var currentExecutable = Assembly.GetExecutingAssembly().Location;

            profilePath = Path.GetDirectoryName(currentExecutable);
        }

        public string LocateFile()
        {
            return $"{profilePath}\\VBoxHeadlessAutoStart.xml";
        }
    }
}