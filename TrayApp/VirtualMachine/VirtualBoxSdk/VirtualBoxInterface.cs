using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TrayApp.VirtualMachine.VirtualBoxSdk
{
    public class VirtualBoxInterface : IMachineLocator, IMachineController
    {
        private readonly VirtualBoxProxyFactory factory;
        private readonly string version;

        public VirtualBoxInterface(VirtualBoxProxyFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));

            version = VirtualBoxVersionLocator.GetVersion();
        }

        public IMachineMetadata[] ListMachines()
        {
            using var instance = factory.Create(version);

            return instance.Machines.Select(
                m => new MachineMetadata(m.Uuid, m.Name, m.State, m.LastStateChange, m.SessionName)
            ).ToArray<IMachineMetadata>();
        }

        public bool Start(IMachineMetadata machine, bool headless)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var instance = factory.Create(version);

            return instance.PowerOn(machine.Uuid, headless).CheckForSuccess();
        }

        public bool SaveState(IMachineMetadata machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var instance = factory.Create(version);

            var session = instance.LockMachine(machine.Uuid);

            return session?.SaveState().CheckForSuccess() == true;
        }

        public bool AcpiPowerOff(IMachineMetadata machine, int waitLimitInMilliseconds)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var instance = factory.Create(version);

            var session = instance.LockMachine(machine.Uuid);
            if (session == null)
            {
                return false;
            }

            if (!session.AcpiPowerOff())
            {
                return false;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < waitLimitInMilliseconds && session.IsLocked)
            {
                Thread.Sleep(250);
            }

            return session.Machine.State == VirtualMachineState.PoweredOff;
        }

        public bool PowerOff(IMachineMetadata machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var instance = factory.Create(version);

            var session = instance.LockMachine(machine.Uuid);

            return session?.PowerDown().CheckForSuccess() == true;
        }

        public bool Reset(IMachineMetadata machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var instance = factory.Create(version);

            var session = instance.LockMachine(machine.Uuid);

            return session?.Reset() == true;
        }
    }
}