using System;

namespace CommonLib.VirtualMachine
{
    public interface IMachineController
    {
        bool StartMachine(IMachine machine, bool headless);

        bool SaveState(IMachine machine);

        bool PowerOff(IMachine machine);

        bool AcpiPowerOff(IMachine machine, int waitLimitInMilliseconds, Action onWaitAction);

        bool Reset(IMachine machine);
    }
}