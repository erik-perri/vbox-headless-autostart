namespace TrayApp.VirtualMachine
{
    public interface ILocatorService
    {
        IMachine[] LocateMachines(bool loadMetadata);

        IMachine[] LocateMachines(IMachineFilter filter, bool loadMetadata);
    }
}