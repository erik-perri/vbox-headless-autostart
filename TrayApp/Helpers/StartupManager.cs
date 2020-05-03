using Microsoft.Win32;
using System;

namespace TrayApp.Helpers
{
    public class StartupManager
    {
        private readonly string keyName;
        private readonly string command;

        public StartupManager(string keyName, string value)
        {
            if (string.IsNullOrWhiteSpace(keyName))
            {
                throw new ArgumentNullException(nameof(keyName));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.keyName = keyName;
            this.command = value;
        }

        public bool IsEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (key == null)
            {
                return false;
            }

            var value = key.GetValue(keyName, null, RegistryValueOptions.None);

            return command.Equals(value?.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public bool EnableStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key == null)
            {
                return false;
            }

            key.SetValue(keyName, command);
            return true;
        }

        public bool DisableStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key == null)
            {
                return false;
            }

            key.DeleteValue(keyName);
            return true;
        }
    }
}