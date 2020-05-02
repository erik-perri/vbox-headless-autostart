namespace TrayApp.VirtualMachine
{
    public enum VirtualMachineState
    {
        Unknown = 0,
        Running,
        Starting,
        PoweredOff,
        Aborted,
        Saving,
        Saved,
        Restoring,
    };
}