namespace TrayApp.VirtualMachine
{
    public enum MachineState
    {
        Unknown = 0,
        Running,
        Starting,
        PoweredOff,
        Aborted,
        Saving,
        Restoring,
        StateSaved,
    };
}