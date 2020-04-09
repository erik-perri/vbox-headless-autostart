namespace TrayApp.VirtualMachine
{
    public interface ILocatorService
    {
        IMachine[] LocateMachines(bool loadMetadata);
    }
}