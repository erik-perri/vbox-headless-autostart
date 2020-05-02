using System;

namespace TrayApp.VirtualMachine.VirtualBoxSdk.Proxy
{
    public interface IMachineProxy
    {
        string Uuid { get; }

        string Name { get; }

        VirtualMachineState State { get; }

        DateTime LastStateChange { get; }

        string SessionName { get; }
    }
}