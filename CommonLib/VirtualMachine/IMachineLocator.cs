namespace CommonLib.VirtualMachine
{
    public interface IMachineLocator
    {
        IMachine[] ListMachines(IMachineFilter filter = null);

        IMachine[] ListMachinesWithMetadata(IMachineFilter filter = null);
    }
}