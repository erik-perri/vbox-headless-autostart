using Microsoft.Win32;
using System;

namespace TrayApp.VirtualMachine
{
    public static class InstallPathLocator
    {
        public static string FindInstallPath()
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Oracle\VirtualBox");

            if (key == null)
            {
                throw new InvalidOperationException("Failed to find VirtualBox install location");
            }

            return (string)key.GetValue("InstallDir", null, RegistryValueOptions.None);
        }
    }
}