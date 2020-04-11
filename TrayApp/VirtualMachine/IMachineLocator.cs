namespace TrayApp.VirtualMachine
{
    public interface IMachineLocator
    {
        IMachine[] LocateMachines(IMachineFilter filter, bool loadMetadata);

        IMachine[] LocateMachines(bool loadMetadata);
    }
}