using System;

namespace TrayApp.VirtualMachine.VirtualBoxSdk
{
    public interface IVirtualBoxInterface : IMachineController, IMachineLocator, IDisposable
    {
    }
}