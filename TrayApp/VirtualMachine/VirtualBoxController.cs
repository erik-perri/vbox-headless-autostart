using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

// TODO Investigate using 'tlbimp /namespace:VirtualBox616 /out:VirtualBox616.dll VirtualBox.tlb' to make versioned
//      namespaces to support multiple versions of VirtualBox

namespace TrayApp.VirtualMachine
{
    public class VirtualBoxController : IMachineController, IMachineLocator
    {
        private readonly ILogger<VirtualBoxController> logger;
        private readonly VirtualBox.VirtualBox virtualbox;

        public VirtualBoxController(ILogger<VirtualBoxController> logger, VirtualBox.VirtualBox virtualBox)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.virtualbox = virtualBox ?? throw new ArgumentNullException(nameof(virtualBox));
        }

        public IMachineMetadata[] ListMachines(IMachineFilter filter = null)
        {
            List<IMachineMetadata> metadata = new List<IMachineMetadata>();

            foreach (var vboxMachine in virtualbox.Machines)
            {
                if (filter?.IncludeMachine(vboxMachine.Id) == false)
                {
                    continue;
                }

                metadata.Add(new MachineMetadata(
                    vboxMachine.Id,
                    vboxMachine.Name,
                    ConvertVirtualBoxState(vboxMachine.State),
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        .AddMilliseconds(vboxMachine.LastStateChange)
                        .ToLocalTime(),
                    vboxMachine.SessionName
                ));
            }

            return metadata.ToArray();
        }

        private MachineState ConvertVirtualBoxState(VirtualBox.MachineState vboxState)
        {
            switch (vboxState)
            {
                case VirtualBox.MachineState.MachineState_Running: return MachineState.Running;
                case VirtualBox.MachineState.MachineState_Starting: return MachineState.Starting;
                case VirtualBox.MachineState.MachineState_PoweredOff: return MachineState.PoweredOff;
                case VirtualBox.MachineState.MachineState_Aborted: return MachineState.Aborted;
                case VirtualBox.MachineState.MachineState_Saving: return MachineState.Saving;
                case VirtualBox.MachineState.MachineState_Saved: return MachineState.Saved;
                case VirtualBox.MachineState.MachineState_Restoring:
                    return MachineState.Restoring;

                default:
                    logger.LogWarning($"Unhandled machine state encountered {new { State = vboxState }}");

                    return MachineState.Unknown;
            }
        }

        public bool StartMachine(IMachineMetadata machine, bool headless)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            logger.LogInformation($"Starting {machine} {new { Headless = headless }}");

            var vboxMachine = virtualbox.FindMachine(machine.Uuid);
            var session = new VirtualBox.Session();

            try
            {
                var frontend = headless ? "headless" : "gui";
                var progress = vboxMachine.LaunchVMProcess(session, frontend, Array.Empty<string>());

                progress.WaitForCompletion(-1);

                session.UnlockMachine();

                if (progress.ResultCode == 0)
                {
                    return true;
                }

                logger.LogError($"LaunchVMProcess returned {progress.ResultCode}, {progress.ErrorInfo.Text}");
            }
            catch (COMException e)
            {
                logger.LogError(e, "COM exception caught while starting machine");
            }

            return false;
        }

        public bool SaveState(IMachineMetadata machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            logger.LogInformation($"Saving state {machine}");

            var vboxMachine = virtualbox.FindMachine(machine.Uuid);
            var session = new VirtualBox.Session();

            try
            {
                vboxMachine.LockMachine(session, VirtualBox.LockType.LockType_Shared);

                var progress = session.Machine.SaveState();

                progress.WaitForCompletion(-1);

                if (progress.ResultCode == 0)
                {
                    return true;
                }

                logger.LogError($"SaveState returned {progress.ResultCode}, {progress.ErrorInfo.Text}");
            }
            catch (COMException e)
            {
                logger.LogError(e, "COM exception caught while saving state");
            }

            return false;
        }

        public bool AcpiPowerOff(IMachineMetadata machine, int waitLimitInMilliseconds, Action onWaitAction)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            logger.LogInformation($"Powering off (ACPI) {machine}");

            var vboxMachine = virtualbox.FindMachine(machine.Uuid);
            var session = new VirtualBox.Session();

            try
            {
                vboxMachine.LockMachine(session, VirtualBox.LockType.LockType_Shared);

                session.Console.PowerButton();

                if (session.Console.GetGuestEnteredACPIMode() < 1)
                {
                    throw new InvalidOperationException("Guest does not support ACPI operations");
                }

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (stopwatch.ElapsedMilliseconds < waitLimitInMilliseconds
                    && session.State == VirtualBox.SessionState.SessionState_Locked)
                {
                    onWaitAction();
                    Thread.Sleep(500);
                }

                return vboxMachine.State == VirtualBox.MachineState.MachineState_PoweredOff;
            }
            catch (COMException e)
            {
                logger.LogError(e, "COM exception caught while powering off (ACPI)");
            }

            return false;
        }

        public bool PowerOff(IMachineMetadata machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            logger.LogInformation($"Powering off {machine}");

            var vboxMachine = virtualbox.FindMachine(machine.Uuid);
            var session = new VirtualBox.Session();

            try
            {
                vboxMachine.LockMachine(session, VirtualBox.LockType.LockType_Shared);

                var progress = session.Console.PowerDown();

                progress.WaitForCompletion(-1);

                if (progress.ResultCode == 0)
                {
                    return true;
                }

                logger.LogError($"PowerDown returned {progress.ResultCode}, {progress.ErrorInfo.Text}");
            }
            catch (COMException e)
            {
                logger.LogError(e, "COM exception caught while powering off");
            }

            return false;
        }

        public bool Reset(IMachineMetadata machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            logger.LogInformation($"Resetting {machine}");

            var vboxMachine = virtualbox.FindMachine(machine.Uuid);
            var session = new VirtualBox.Session();

            try
            {
                vboxMachine.LockMachine(session, VirtualBox.LockType.LockType_Shared);

                session.Console.Reset();

                return true;
            }
            catch (COMException e)
            {
                logger.LogError(e, "COM exception caught while powering off");
            }

            return false;
        }
    }
}