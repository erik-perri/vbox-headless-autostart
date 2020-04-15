using System;

namespace CommonLib.VirtualMachine
{
    public interface IMachineController
    {
        bool StartMachine(IMachineMetadata machine, bool headless);

        bool SaveState(IMachineMetadata machine);

        bool PowerOff(IMachineMetadata machine);

        bool AcpiPowerOff(IMachineMetadata machine, int waitLimitInMilliseconds, Action onWaitAction);

        bool Reset(IMachineMetadata machine);
    }
}