namespace TrayApp.VirtualMachine
{
    public enum MachineState
    {
        Unknown = 0,
        Running,
        PoweredOff,
        Aborted,
        Saving,
        Restoring,
        StateSaved,
    };
}