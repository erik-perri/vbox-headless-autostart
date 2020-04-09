using System;
using System.Diagnostics;
using System.IO;

namespace TrayApp.VirtualMachine.VirtualBox
{
    public class VBoxManageProcess : Process
    {
        public VBoxManageProcess(string arguments)
        {
            var installPath = InstallPathLocator.FindInstallPath();
            if (installPath == null || !Directory.Exists(installPath))
            {
                throw new InvalidOperationException("VirtualBox path not found");
            }

            StartInfo = new ProcessStartInfo()
            {
                FileName = $"{installPath}\\VBoxManage.exe",
                Arguments = arguments,
                CreateNoWindow = true,
            };
        }
    }
}