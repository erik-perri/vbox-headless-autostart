using CommonLib.VirtualMachine;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using TrayApp.Helpers;

namespace TrayApp.VirtualMachine.VirtualBox
{
    public class MachineController : IMachineController
    {
        private readonly ILogger<MachineController> logger;
        private readonly MetadataReader metadataReader;

        public MachineController(ILogger<MachineController> logger, MetadataReader metadataReader)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.metadataReader = metadataReader ?? throw new ArgumentNullException(nameof(metadataReader));
        }

        public bool StartMachineHeadless(IMachine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var process = new VBoxManageProcess($"startvm {machine.Uuid} --type headless");
            var output = process.GetOuput();

            foreach (var line in output.OutputData)
            {
                if (Regex.Match(line, @"VM "".*"" has been successfully started.").Success)
                {
                    return true;
                }
            }

            return false;
        }

        public bool StartMachine(IMachine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var process = new VBoxManageProcess($"startvm {machine.Uuid}");
            var output = process.GetOuput();

            foreach (var line in output.OutputData)
            {
                if (Regex.Match(line, @"VM "".*"" has been successfully started.").Success)
                {
                    return true;
                }
            }

            return false;
        }

        public bool SaveState(IMachine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var process = new VBoxManageProcess($"controlvm {machine.Uuid} savestate");
            var output = process.GetOuput(-1, -35, logger);

            foreach (var line in output.OutputData)
            {
                if (line.Contains("100%", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public bool PowerOff(IMachine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var process = new VBoxManageProcess($"controlvm {machine.Uuid} poweroff");
            var output = process.GetOuput(-1, -35, logger);

            foreach (var line in output.OutputData)
            {
                if (Regex.Match(line, @"VM "".*"" has been successfully started.").Success)
                {
                    return true;
                }
            }

            return false;
        }

        public bool AcpiPowerOff(IMachine machine, int waitLimitInMilliseconds, Action onWaitAction)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var process = new VBoxManageProcess($"controlvm {machine.Uuid} acpipowerbutton");

            process.Start();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var waitTimeSpan = TimeSpan.FromMilliseconds(waitLimitInMilliseconds);
            while (metadataReader.ReadMetadata(machine).State == MachineState.Running && stopwatch.Elapsed < waitTimeSpan)
            {
                onWaitAction();
                Thread.Sleep(500);
            }

            stopwatch.Stop();

            if (metadataReader.ReadMetadata(machine).State == MachineState.Running)
            {
                logger.LogError($"Timed out while powering off {new { machine.Uuid, machine.Name }}");
                return false;
            }

            return true;
        }

        public void Reset(IMachine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var process = new VBoxManageProcess($"controlvm {machine.Uuid} reset");

            process.Start();
        }
    }
}