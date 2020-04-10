namespace TrayApp.VirtualMachine
{
    public interface ILocatorService
    {
        IMachine[] LocateMachines(IMachineFilter filter, bool loadMetadata);

        IMachine[] LocateMachines(bool loadMetadata);
    }
}