using System;
using System.Collections.Generic;

namespace TrayApp.VirtualMachine.VirtualBoxSdk.Proxy
{
    public interface IVirtualBoxProxy : IDisposable
    {
        IEnumerable<IMachineProxy> Machines { get; }

        IProgressProxy PowerOn(string uuid, bool headless);

        ISessionProxy LockMachine(string uuid);
    }
}