namespace TrayApp.VirtualMachine
{
    public interface IMachine
    {
        string Uuid { get; }

        string Name { get; }

        IMachineMetadata Metadata { get; set; }
    }
}