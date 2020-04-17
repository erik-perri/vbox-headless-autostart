using System;

namespace TrayApp.VirtualMachine
{
    public interface IMachineController
    {
        bool Start(IMachineMetadata machine, bool headless);

        bool SaveState(IMachineMetadata machine);

        bool PowerOff(IMachineMetadata machine);

        bool AcpiPowerOff(IMachineMetadata machine, int waitLimitInMilliseconds, Action onWaitAction);

        bool Reset(IMachineMetadata machine);
    }
}