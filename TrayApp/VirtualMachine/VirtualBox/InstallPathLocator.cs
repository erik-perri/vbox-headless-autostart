using Microsoft.Win32;

namespace TrayApp.VirtualMachine.VirtualBox
{
    public static class InstallPathLocator
    {
        public static string FindInstallPath()
        {
            return (string)Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Oracle\VirtualBox")
                .GetValue("InstallDir", null, RegistryValueOptions.None);
        }
    }
}