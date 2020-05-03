using System;
using VirtualBox60;

namespace TrayApp.VirtualMachine.VirtualBoxSdk.Proxy.Version60
{
    public class MachineProxy : IMachineProxy
    {
        private readonly IMachine machine;

        public MachineProxy(IMachine machine)
        {
            this.machine = machine ?? throw new ArgumentNullException(nameof(machine));
        }

        public string Uuid => machine.Id;

        public string Name => machine.Name;

        public VirtualMachineState State => machine.State switch
        {
            MachineState.MachineState_Running => VirtualMachineState.Running,
            MachineState.MachineState_Starting => VirtualMachineState.Starting,
            MachineState.MachineState_PoweredOff => VirtualMachineState.PoweredOff,
            MachineState.MachineState_Aborted => VirtualMachineState.Aborted,
            MachineState.MachineState_Saving => VirtualMachineState.Saving,
            MachineState.MachineState_Saved => VirtualMachineState.Saved,
            MachineState.MachineState_Restoring => VirtualMachineState.Restoring,
            _ => VirtualMachineState.Unknown
        };

        public DateTime LastStateChange =>
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(machine.LastStateChange)
                .ToLocalTime();

        public string SessionName => machine.SessionName;
    }
}