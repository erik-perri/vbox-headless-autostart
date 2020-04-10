using System;

namespace TrayApp.VirtualMachine
{
    public interface IMachineController
    {
        bool StartMachineHeadless(IMachine machine);

        bool StartMachine(IMachine machine);

        bool SaveState(IMachine machine);

        bool PowerOff(IMachine machine);

        bool AcpiPowerOff(IMachine machine, int waitLimitInMilliseconds, Action onWaitAction);

        void Reset(IMachine machine);
    }
}