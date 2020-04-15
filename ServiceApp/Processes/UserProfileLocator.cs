using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CommonLib.Processes
{
    public class UserProfileLocator
    {
        private readonly ILogger<UserProfileLocator> logger;

        public UserProfileLocator(ILogger<UserProfileLocator> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string LocatePathFromProcess(Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (!NativeMethods.OpenProcessToken(process.Handle, NativeMethods.TOKEN_QUERY, out var tokenHandle))
            {
                logger.LogError($"Failed to open process {process.Id} token, error: {Marshal.GetLastWin32Error()}");
                return null;
            }

            using (tokenHandle)
            {
                int size = 0;

                // Get the expected size
                NativeMethods.GetUserProfileDirectory(tokenHandle, null, ref size);

                StringBuilder profilePath = new StringBuilder(size);
                if (!NativeMethods.GetUserProfileDirectory(tokenHandle, profilePath, ref size))
                {
                    logger.LogError($"Failed to get profile directory, error: {Marshal.GetLastWin32Error()}");
                    return null;
                }

                return profilePath.ToString();
            }
        }

        private static class NativeMethods
        {
            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess,
                                                         out SafeAccessTokenHandle TokenHandle);

            public const uint TOKEN_QUERY = 0x0008;

            [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool GetUserProfileDirectory(SafeAccessTokenHandle hToken, StringBuilder path,
                                                              ref int dwSize);
        }
    }
}