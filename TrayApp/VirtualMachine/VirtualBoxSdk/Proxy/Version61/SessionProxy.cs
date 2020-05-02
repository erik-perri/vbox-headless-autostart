using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using VirtualBox61;

namespace TrayApp.VirtualMachine.VirtualBoxSdk.Proxy.Version61
{
    public class SessionProxy : ISessionProxy
    {
        private readonly ILogger logger;
        private readonly Session session;

        public SessionProxy(ILogger logger, Session session)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public bool IsLocked => session?.State == SessionState.SessionState_Locked;

        public IMachineProxy Machine => new MachineProxy(session?.Machine);

        public IProgressProxy SaveState()
        {
            try
            {
                return new ProgressProxy(logger, session.Machine.SaveState());
            }
            catch (COMException e)
            {
                logger.LogError(e, "COM exception caught in SaveState()");
            }

            return null;
        }

        public IProgressProxy PowerDown()
        {
            try
            {
                return new ProgressProxy(logger, session.Console.PowerDown());
            }
            catch (COMException e)
            {
                logger.LogError(e, "COM exception caught in PowerDown()");
            }

            return null;
        }

        public bool AcpiPowerOff()
        {
            if (session == null) return false;

            try
            {
                session.Console.PowerButton();

                if (session.Console.GetGuestEnteredACPIMode() > 0) return true;
            }
            catch (COMException e)
            {
                logger.LogError(e, "COM exception caught in AcpiPowerOff()");
            }

            return false;
        }

        public bool Reset()
        {
            if (session == null) return false;

            try
            {
                session.Console.Reset();

                return true;
            }
            catch (COMException e)
            {
                logger.LogError(e, "COM exception caught in Reset()");
            }

            return false;
        }
    }
}