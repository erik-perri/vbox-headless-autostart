using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using VirtualBox60;

namespace TrayApp.VirtualMachine.VirtualBoxSdk.Proxy.Version60
{
    public class VirtualBoxProxy : IVirtualBoxProxy
    {
        private readonly VirtualBox instance;
        private readonly ILogger<VirtualBoxProxy> logger;

        public VirtualBoxProxy(ILogger<VirtualBoxProxy> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            instance = new VirtualBox();
        }

        public IEnumerable<IMachineProxy> Machines =>
            instance.Machines.Select(m => new MachineProxy(m)).ToList<IMachineProxy>();

        public IProgressProxy PowerOn(string uuid, bool headless)
        {
            try
            {
                var machine = instance.FindMachine(uuid);
                if (machine == null)
                {
                    return null;
                }

                var session = new Session();

                var frontend = headless ? "headless" : "gui";
                var progress = machine.LaunchVMProcess(session, frontend, "");

                Task.Run(() =>
                {
                    progress.WaitForCompletion(-1);
                    session.UnlockMachine();
                });

                return new ProgressProxy(logger, progress);
            }
            catch (COMException e)
            {
                logger.LogError(e, $"COM exception caught in PowerOn({uuid}, {headless})");
            }

            return null;
        }

        public ISessionProxy LockMachine(string uuid)
        {
            try
            {
                var machine = instance.FindMachine(uuid);
                if (machine == null)
                {
                    return null;
                }

                var session = new Session();

                machine.LockMachine(session, LockType.LockType_Shared);

                return new SessionProxy(logger, session);
            }
            catch (COMException e)
            {
                logger.LogError(e, $"COM exception caught in LockMachine({uuid})");
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Marshal.ReleaseComObject(instance);
        }
    }
}