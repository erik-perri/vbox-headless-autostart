using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace ServiceApp.Processes.SafeHandles
{
    public sealed class SafeEnvironmentHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeEnvironmentHandle()
            : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle() => NativeMethods.DestroyEnvironmentBlock(handle);

        private static class NativeMethods
        {
            [DllImport("userenv.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);
        }
    }
}