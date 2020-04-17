using System;

namespace TrayApp.VirtualMachine.VirtualBoxSdk
{
    public class InvalidInstallException : Exception
    {
        public InvalidInstallException() : base()
        {
        }

        public InvalidInstallException(string message) : base(message)
        {
        }

        public InvalidInstallException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}