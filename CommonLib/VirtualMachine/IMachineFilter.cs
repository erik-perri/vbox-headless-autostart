namespace CommonLib.VirtualMachine
{
    public interface IMachineFilter
    {
        bool IncludeMachine(string uuid);
    }
}