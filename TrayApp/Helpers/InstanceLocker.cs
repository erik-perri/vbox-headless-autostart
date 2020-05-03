using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;

namespace TrayApp.Helpers
{
    public class InstanceLocker : IDisposable
    {
        private readonly ILogger<InstanceLocker> logger;
        private Mutex mutex;

        public InstanceLocker(ILogger<InstanceLocker> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool StartLock()
        {
            var mutexName = $"Local\\{Assembly.GetEntryAssembly()?.GetName().Name}";

            mutex = new Mutex(true, mutexName, out var createdNew);

            logger.LogDebug(
                createdNew
                    ? $"Mutex created \"{mutexName}\""
                    : $"Mutex already exists \"{mutexName}\""
            );

            return createdNew;
        }

        public void StopLock()
        {
            if (mutex != null)
            {
                logger.LogDebug("Destroying mutex");
                mutex.ReleaseMutex();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mutex?.Dispose();
            }
        }
    }
}