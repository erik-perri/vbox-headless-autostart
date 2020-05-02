namespace TrayApp.VirtualMachine.VirtualBoxSdk.Proxy
{
    public interface ISessionProxy
    {
        bool IsLocked { get; }

        IMachineProxy Machine { get; }

        IProgressProxy SaveState();

        IProgressProxy PowerDown();

        bool AcpiPowerOff();

        bool Reset();
    }
}