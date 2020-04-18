namespace TrayApp.VirtualMachine
{
    public interface IMachineLocator
    {
        IMachineMetadata[] ListMachines();
    }
}