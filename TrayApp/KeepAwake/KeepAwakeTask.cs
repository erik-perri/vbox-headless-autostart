using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace TrayApp.KeepAwake
{
    public class KeepAwakeTask : IDisposable
    {
        private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);
        private Task keepAwakeTask;

        private readonly ILogger<KeepAwakeTask> logger;

        public KeepAwakeTask(ILogger<KeepAwakeTask> logger)
        {
            logger.LogTrace(".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsRunning
        {
            get
            {
                if (keepAwakeTask == null)
                {
                    return false;
                }

                return keepAwakeTask.Status.Equals(TaskStatus.Running)
                    || keepAwakeTask.Status.Equals(TaskStatus.WaitingToRun);
            }
        }

        public void Start()
        {
            if (keepAwakeTask != null)
            {
                throw new InvalidOperationException("Keep awake task already running");
            }

            logger.LogTrace("Start keep awake task");

            keepAwakeTask = Task.Factory.StartNew(() =>
            {
                // Tell Windows we don't want it to sleep as long as this thread is running
                var previousExecutionState = NativeMethods.SetThreadExecutionState(
                    NativeMethods.EXECUTION_STATE.ES_CONTINUOUS | NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED
                );
                if (previousExecutionState == 0)
                {
                    throw new Win32Exception();
                }

                waitEvent.WaitOne();

                _ = NativeMethods.SetThreadExecutionState((NativeMethods.EXECUTION_STATE)previousExecutionState);

                logger.LogTrace("Keep awake task finished");
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Stop()
        {
            logger.LogTrace("Stopping keep awake task");

            waitEvent.Set();
            keepAwakeTask.Wait();
            keepAwakeTask.Dispose();
            keepAwakeTask = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            logger.LogTrace($"Dispose({disposing})");

            if (disposing)
            {
                if (keepAwakeTask?.IsCompleted == false)
                {
                    waitEvent.Set();
                    keepAwakeTask.Wait();
                }

                keepAwakeTask?.Dispose();
                waitEvent?.Dispose();
            }
        }

        private static class NativeMethods
        {
            // https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setthreadexecutionstate
            [Flags]
#pragma warning disable RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
            public enum EXECUTION_STATE : uint
#pragma warning restore RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
            {
                ES_SYSTEM_REQUIRED = 0x00000001,
                ES_DISPLAY_REQUIRED = 0x00000002,
                ES_AWAYMODE_REQUIRED = 0x00000040,
                ES_CONTINUOUS = 0x80000000,
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern uint SetThreadExecutionState(EXECUTION_STATE esFlags);
        }
    }
}