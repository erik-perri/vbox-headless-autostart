using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TrayApp.AutoControl
{
    public class ShutdownBlock : IDisposable
    {
        private readonly ILogger<ShutdownBlock> logger;
        private readonly HandleRef handle;

        public ShutdownBlock(ILogger<ShutdownBlock> logger, Control window)
        {
            logger.LogTrace($".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.handle = new HandleRef(window ?? throw new ArgumentNullException(nameof(window)), window.Handle);
        }

        public bool StartBlocking(string reason)
        {
            if (GetBlockReason() != null)
            {
                StopBlocking();
            }

            if (!NativeMethods.ShutdownBlockReasonCreate(handle.Handle, reason))
            {
                logger.LogError($"Failed to create shutdown block {new { LastError = Marshal.GetLastWin32Error() }}");
                return false;
            }

            logger.LogDebug($"Created shutdown block {new { Reason = reason }}");
            return true;
        }

        public bool StopBlocking()
        {
            if (GetBlockReason() != null && !NativeMethods.ShutdownBlockReasonDestroy(handle.Handle))
            {
                logger.LogError($"Failed to destroy shutdown block {new { LastError = Marshal.GetLastWin32Error() }}");
                return false;
            }

            return true;
        }

        public string GetBlockReason()
        {
            uint bufferSize = 0;

            if (!NativeMethods.ShutdownBlockReasonQuery(handle.Handle, null, ref bufferSize))
            {
                logger.LogError($"Failed to retrieve buffer size {new { LastError = Marshal.GetLastWin32Error() }}");
                return null;
            }

            var builder = new StringBuilder((int)bufferSize);
            if (!NativeMethods.ShutdownBlockReasonQuery(handle.Handle, builder, ref bufferSize))
            {
                logger.LogError($"Failed to retrieve reason {new { LastError = Marshal.GetLastWin32Error() }}");
                return null;
            }

            return builder.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopBlocking();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string reason);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool ShutdownBlockReasonQuery(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder reason, ref uint size);
        }
    }
}