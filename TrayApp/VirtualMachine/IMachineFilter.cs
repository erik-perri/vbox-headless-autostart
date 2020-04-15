namespace TrayApp.VirtualMachine
{
    public interface IMachineFilter
    {
        bool IncludeMachine(string uuid);
    }
}