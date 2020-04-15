namespace CommonLib.VirtualMachine
{
    public interface IMachineLocator
    {
        IMachineMetadata[] ListMachines(IMachineFilter filter = null);
    }
}