using Microsoft.Win32;
using System;

namespace TrayApp.VirtualMachine.VirtualBoxSdk
{
    public static class VirtualBoxVersionLocator
    {
        public static string GetVersion()
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Oracle\VirtualBox");
            if (key == null)
            {
                throw new InvalidInstallException("VirtualBox install not found");
            }

            var value = (string)key.GetValue("Version", null, RegistryValueOptions.None);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value;
        }

        public static Version ParseVersion()
        {
            var version = GetVersion();
            if (version == null)
            {
                return null;
            }

            return Version.Parse(version);
        }
    }
}