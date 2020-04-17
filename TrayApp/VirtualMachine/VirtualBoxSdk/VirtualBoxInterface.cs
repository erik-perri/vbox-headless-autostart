﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace TrayApp.VirtualMachine.VirtualBoxSdk
{
    public class VirtualBoxInterface : IDisposable, IMachineLocator, IMachineController
    {
        private readonly ILogger<VirtualBoxInterface> logger;
        private readonly VirtualBox616.IVirtualBox instance;

        public ReadOnlyCollection<VirtualBox616.IMachine> Machines { get; }

        public VirtualBoxInterface(ILogger<VirtualBoxInterface> logger)
        {
            this.logger = logger;
            instance = new VirtualBox616.VirtualBox();
            Machines = new ReadOnlyCollection<VirtualBox616.IMachine>(instance.Machines);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Marshal.ReleaseComObject(instance);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IMachineMetadata[] ListMachines(IMachineFilter filter = null)
        {
            var metadata = new List<IMachineMetadata>();

            foreach (var vboxMachine in instance.Machines)
            {
                if (filter?.IncludeMachine(vboxMachine.Id) == false)
                {
                    continue;
                }

                metadata.Add(new MachineMetadata(
                    vboxMachine.Id,
                    vboxMachine.Name,
                    ConvertVirtualBoxState(vboxMachine.State),
                    // The VirtualBox last change is stored as milliseconds since the unix epoch
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        .AddMilliseconds(vboxMachine.LastStateChange)
                        .ToLocalTime(),
                    vboxMachine.SessionName
                ));
            }

            return metadata.ToArray();
        }

        private MachineState ConvertVirtualBoxState(VirtualBox616.MachineState vboxState)
        {
            switch (vboxState)
            {
                case VirtualBox616.MachineState.MachineState_Running: return MachineState.Running;
                case VirtualBox616.MachineState.MachineState_Starting: return MachineState.Starting;
                case VirtualBox616.MachineState.MachineState_PoweredOff: return MachineState.PoweredOff;
                case VirtualBox616.MachineState.MachineState_Aborted: return MachineState.Aborted;
                case VirtualBox616.MachineState.MachineState_Saving: return MachineState.Saving;
                case VirtualBox616.MachineState.MachineState_Saved: return MachineState.Saved;
                case VirtualBox616.MachineState.MachineState_Restoring:
                    return MachineState.Restoring;

                default:
                    logger.LogWarning($"Unhandled machine state encountered {new { State = vboxState }}");

                    return MachineState.Unknown;
            }
        }

        public bool Start(IMachineMetadata machine, bool headless)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            logger.LogInformation($"Starting {machine} {new { Headless = headless }}");

            var vboxMachine = instance.FindMachine(machine.Uuid);
            var session = new VirtualBox616.Session();

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

            var vboxMachine = instance.FindMachine(machine.Uuid);
            var session = new VirtualBox616.Session();

            try
            {
                vboxMachine.LockMachine(session, VirtualBox616.LockType.LockType_Shared);

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

        public bool PowerOff(IMachineMetadata machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            logger.LogInformation($"Powering off {machine}");

            var vboxMachine = instance.FindMachine(machine.Uuid);
            var session = new VirtualBox616.Session();

            try
            {
                vboxMachine.LockMachine(session, VirtualBox616.LockType.LockType_Shared);

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

        public bool AcpiPowerOff(IMachineMetadata machine, int waitLimitInMilliseconds, Action onWaitAction)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            logger.LogInformation($"Powering off (ACPI) {machine}");

            var vboxMachine = instance.FindMachine(machine.Uuid);
            var session = new VirtualBox616.Session();

            try
            {
                vboxMachine.LockMachine(session, VirtualBox616.LockType.LockType_Shared);

                session.Console.PowerButton();

                if (session.Console.GetGuestEnteredACPIMode() < 1)
                {
                    throw new InvalidOperationException("Guest does not support ACPI operations");
                }

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (stopwatch.ElapsedMilliseconds < waitLimitInMilliseconds
                    && session.State == VirtualBox616.SessionState.SessionState_Locked)
                {
                    onWaitAction();
                    Thread.Sleep(250);
                }

                return vboxMachine.State == VirtualBox616.MachineState.MachineState_PoweredOff;
            }
            catch (COMException e)
            {
                logger.LogError(e, "COM exception caught while powering off (ACPI)");
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

            var vboxMachine = instance.FindMachine(machine.Uuid);
            var session = new VirtualBox616.Session();

            try
            {
                vboxMachine.LockMachine(session, VirtualBox616.LockType.LockType_Shared);

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