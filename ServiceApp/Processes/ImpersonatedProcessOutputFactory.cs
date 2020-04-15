using CommonLib.Processes;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using ServiceApp.Processes.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

// https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-createprocessasusera
// https://docs.microsoft.com/en-us/windows/win32/procthread/creating-a-child-process-with-redirected-input-and-output
namespace ServiceApp.Processes
{
    public class ImpersonatedProcessOutputFactory : IProcessOutputFactory, IDisposable
    {
        private readonly ILogger<ImpersonatedProcessOutputFactory> logger;
        private SafeAccessTokenHandle impersonationToken = new SafeAccessTokenHandle(IntPtr.Zero);

        public ImpersonatedProcessOutputFactory(ILogger<ImpersonatedProcessOutputFactory> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool ImpersonateUserFromProcess(Process process)
        {
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (!NativeMethods.OpenProcessToken(process.Handle, NativeMethods.TOKEN_DUPLICATE, out var processToken))
            {
                logger.LogError($"Failed to open process {process.Id} token, error: {Marshal.GetLastWin32Error()}");
                return false;
            }

            using (processToken)
            {
                var securityAttributes = new NativeMethods.SECURITY_ATTRIBUTES();
                securityAttributes.nLength = (uint)Marshal.SizeOf(securityAttributes);

                if (!NativeMethods.DuplicateTokenEx(
                    processToken,
                    NativeMethods.TOKEN_ASSIGN_PRIMARY | NativeMethods.TOKEN_DUPLICATE | NativeMethods.TOKEN_QUERY,
                    ref securityAttributes,
                    NativeMethods.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                    NativeMethods.TOKEN_TYPE.TokenPrimary,
                    out var duplicatedToken
                ))
                {
                    logger.LogError(
                        $"Failed to duplicate process {process.Id} token, error: {Marshal.GetLastWin32Error()}"
                    );
                    return false;
                }

                impersonationToken = duplicatedToken;
                return true;
            }
        }

        public IProcessOutput CreateProcess(string fileName, string arguments)
        {
            if (impersonationToken.IsInvalid)
            {
                throw new InvalidOperationException("ImpersonateUserFromProcess must be called first");
            }

            if (!NativeMethods.CreateEnvironmentBlock(out var environment, impersonationToken, false))
            {
                throw new Win32Exception();
            }

            using (environment)
            {
                return CreateProcessAsUser(impersonationToken, environment, fileName, arguments);
            }
        }

        private IProcessOutput CreateProcessAsUser(
            SafeAccessTokenHandle impersonationToken,
            SafeEnvironmentHandle environment,
            string fileName,
            string arguments
        )
        {
            var startupInfo = new NativeMethods.STARTUPINFO();
            startupInfo.cb = (uint)Marshal.SizeOf(startupInfo);

            var pipeSecurityAttributes = new NativeMethods.SECURITY_ATTRIBUTES();
            pipeSecurityAttributes.nLength = (uint)Marshal.SizeOf(pipeSecurityAttributes);
            pipeSecurityAttributes.lpSecurityDescriptor = IntPtr.Zero;
            pipeSecurityAttributes.bInheritHandle = true;

            if (!NativeMethods.CreatePipe(
                out var readStandardOutputPipePointer,
                out var childStandardOutputPipePointer,
                ref pipeSecurityAttributes,
                0 // Let the system choose the buffer size
            ))
            {
                throw new Win32Exception();
            }

            using var readStandardOutputPipe = new SafeFileHandle(readStandardOutputPipePointer, true);
            using var childStandardOutputPipe = new SafeFileHandle(childStandardOutputPipePointer, true);

            if (!NativeMethods.SetHandleInformation(readStandardOutputPipe, NativeMethods.HANDLE_FLAG_INHERIT, 0))
            {
                throw new Win32Exception();
            }

            startupInfo.hStdOutput = childStandardOutputPipe.DangerousGetHandle();

            if (!NativeMethods.CreatePipe(
                out var readStandardErrorPipePointer,
                out var childStandardErrorPipePointer,
                ref pipeSecurityAttributes,
                0 // Let the system choose the buffer size
            ))
            {
                throw new Win32Exception();
            }

            using var readStandardErrorPipe = new SafeFileHandle(readStandardErrorPipePointer, true);
            using var childStandardErrorPipe = new SafeFileHandle(childStandardErrorPipePointer, true);

            if (!NativeMethods.SetHandleInformation(readStandardErrorPipe, NativeMethods.HANDLE_FLAG_INHERIT, 0))
            {
                throw new Win32Exception();
            }

            startupInfo.hStdError = childStandardErrorPipe.DangerousGetHandle();

            // Tell Windows we want interactive access to the desktop
            // https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-createprocessasusera
            startupInfo.lpDesktop = @"winsta0\default";

            startupInfo.wShowWindow = NativeMethods.SW_HIDE;
            startupInfo.dwFlags = NativeMethods.STARTF_USESHOWWINDOW | NativeMethods.STARTF_USESTDHANDLES;

            var processSecurityAttributes = new NativeMethods.SECURITY_ATTRIBUTES();
            var threadSecurityAttributes = new NativeMethods.SECURITY_ATTRIBUTES();
            processSecurityAttributes.nLength = (uint)Marshal.SizeOf(processSecurityAttributes);
            threadSecurityAttributes.nLength = (uint)Marshal.SizeOf(threadSecurityAttributes);

            if (!NativeMethods.CreateProcessAsUser(
                impersonationToken,
                null,
                $"\"{fileName}\" {arguments}",
                ref processSecurityAttributes,
                ref threadSecurityAttributes,
                true, // Inherit attributes, this is required for redirecting stdout/stderr
                NativeMethods.CREATE_UNICODE_ENVIRONMENT,
                environment,
                null, // Current directory
                ref startupInfo,
                out var processInformation
            ))
            {
                throw new Win32Exception();
            }

            // Make sure the unneeded handles are closed.
            using var processHandle = new SafeWaitHandle(processInformation.hProcess, true);
            using var threadHandle = new SafeWaitHandle(processInformation.hThread, true);

            // Close the child pipe handles, this must be done before reading from the read pipes or the child hangs
            childStandardOutputPipe.Dispose();
            childStandardErrorPipe.Dispose();

            using var outputStream = new StreamReader(
                new FileStream(readStandardOutputPipe, FileAccess.Read),
                Encoding.ASCII,
                true
            );

            using var errorStream = new StreamReader(
                new FileStream(readStandardErrorPipe, FileAccess.Read),
                Encoding.ASCII,
                true
            );

            var outputContent = outputStream.ReadToEnd();
            var errorContent = errorStream.ReadToEnd();

            // Close the streams after reading them, this must be done before WaitForSingleObject or the child hangs
            readStandardOutputPipe.Dispose();
            readStandardErrorPipe.Dispose();
            outputStream.Dispose();
            errorStream.Dispose();

            var exitCode = WaitForExitCode(processHandle);

            return new ProcessOutput(
                processInformation.dwProcessId,
                exitCode,
                outputContent,
                errorContent
            );
        }

        private static int WaitForExitCode(SafeWaitHandle processHandle, uint waitMilliseconds = 0)
        {
            if (processHandle.IsInvalid)
            {
                throw new InvalidOperationException("Process handle is invalid");
            }

            var waitResult = NativeMethods.WaitForSingleObject(
                processHandle,
                waitMilliseconds == 0
                    ? NativeMethods.INFINITE
                    : waitMilliseconds
            );

            if (waitResult == NativeMethods.WAIT_FAILED)
            {
                throw new Win32Exception();
            }

            if (!NativeMethods.GetExitCodeProcess(processHandle, out var exitCode))
            {
                throw new Win32Exception();
            }

            return (int)exitCode;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !impersonationToken.IsInvalid)
            {
                impersonationToken.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static class NativeMethods
        {
            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess,
                                                         out SafeAccessTokenHandle TokenHandle);

            internal const uint TOKEN_DUPLICATE = 0x0002;

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool DuplicateTokenEx(SafeAccessTokenHandle hExistingToken, uint dwDesiredAccess,
                                                         ref SECURITY_ATTRIBUTES lpTokenAttributes,
                                                         SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
                                                         TOKEN_TYPE TokenType, out SafeAccessTokenHandle phNewToken);

            internal const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
            internal const uint TOKEN_QUERY = 0x0008;

            [StructLayout(LayoutKind.Sequential)]
            internal struct SECURITY_ATTRIBUTES
            {
                public uint nLength;
                public IntPtr lpSecurityDescriptor;
                public bool bInheritHandle;
            }

            internal enum SECURITY_IMPERSONATION_LEVEL
            {
                SecurityAnonymous,
                SecurityIdentification,
                SecurityImpersonation,
                SecurityDelegation
            }

            internal enum TOKEN_TYPE
            {
                TokenPrimary = 1,
                TokenImpersonation
            }

            [DllImport("userenv.dll", SetLastError = true)]
            internal static extern bool CreateEnvironmentBlock(out SafeEnvironmentHandle lpEnvironment,
                                                               SafeAccessTokenHandle hToken, bool bInherit);

            [StructLayout(LayoutKind.Sequential)]
            internal struct PROCESS_INFORMATION
            {
                public IntPtr hProcess;
                public IntPtr hThread;
                public uint dwProcessId;
                public uint dwThreadId;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct STARTUPINFO
            {
                public uint cb;
                public string lpReserved;
                public string lpDesktop;
                public string lpTitle;
                public uint dwX;
                public uint dwY;
                public uint dwXSize;
                public uint dwYSize;
                public uint dwXCountChars;
                public uint dwYCountChars;
                public uint dwFillAttribute;
                public uint dwFlags;
                public short wShowWindow;
                public short cbReserved2;
                public IntPtr lpReserved2;
                public IntPtr hStdInput;
                public IntPtr hStdOutput;
                public IntPtr hStdError;
            }

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern bool CreateProcessAsUser(SafeAccessTokenHandle hToken, string lpApplicationName,
                                                            string lpCommandLine,
                                                            ref SECURITY_ATTRIBUTES lpProcessAttributes,
                                                            ref SECURITY_ATTRIBUTES lpThreadAttributes,
                                                            bool bInheritHandles, uint dwCreationFlags,
                                                            SafeEnvironmentHandle lpEnvironment,
                                                            string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo,
                                                            out PROCESS_INFORMATION lpProcessInformation);

            internal const int STARTF_USESHOWWINDOW = 0x00000001;
            internal const int STARTF_FORCEONFEEDBACK = 0x00000040;
            internal const int STARTF_FORCEOFFFEEDBACK = 0x00000080;
            internal const int STARTF_USESTDHANDLES = 0x00000100;

            internal const short SW_HIDE = 0;
            internal const short SW_SHOW = 5;

            internal const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;

            [DllImport("kernel32.dll")]
            internal static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe,
                                                   ref SECURITY_ATTRIBUTES lpPipeAttributes, uint nSize);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool SetHandleInformation(SafeFileHandle hObject, uint dwMask, uint dwFlags);

            internal const uint HANDLE_FLAG_INHERIT = 0x00000001;
            internal const uint HANDLE_FLAG_PROTECT_FROM_CLOSE = 0x00000002;

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern uint WaitForSingleObject(SafeWaitHandle hHandle, uint dwMilliseconds);

            internal const uint INFINITE = 0xFFFFFFFF;
            internal const uint WAIT_FAILED = 0xFFFFFFFF;

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetExitCodeProcess(SafeWaitHandle hProcess, out uint lpExitCode);

            internal const int STILL_ACTIVE = 259;
        }
    }
}