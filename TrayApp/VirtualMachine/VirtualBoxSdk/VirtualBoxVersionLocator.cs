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

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}