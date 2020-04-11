using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrayApp.Configuration;

namespace TrayApp.VirtualMachine
{
    public class MachineStoreUpdater : IDisposable
    {
        private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);
        private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private readonly ILogger<MachineStoreUpdater> logger;
        private readonly MachineStore machineStore;
        private readonly IUpdateSpeedLocator updateSpeedLocator;

        private Task updateTask;

        public MachineStoreUpdater(
            ILogger<MachineStoreUpdater> logger,
            MachineStore machineStore,
            ConfigurationStore configurationStore,
            IUpdateSpeedLocator updateSpeedLocator
        )
        {
            logger.LogTrace(".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.machineStore = machineStore ?? throw new ArgumentNullException(nameof(machineStore));
            this.updateSpeedLocator = updateSpeedLocator ?? throw new ArgumentNullException(nameof(updateSpeedLocator));

            if (configurationStore == null)
            {
                throw new ArgumentNullException(nameof(configurationStore));
            }

            configurationStore.OnConfigurationChange += (object _, EventArgs __) => RequestUpdate();
        }

        public void RequestUpdate()
        {
            waitEvent.Set();
        }

        public void StartMonitor()
        {
            if (updateTask != null)
            {
                throw new InvalidOperationException("Machine updater already running");
            }

            var cancellationToken = this.cancellationToken.Token;

            updateTask = Task.Factory.StartNew(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    machineStore.UpdateMachines();

                    waitEvent.WaitOne(updateSpeedLocator.GetUpdateSpeed());
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void StopMonitor()
        {
            cancellationToken.Cancel();
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
                if (updateTask?.IsCompleted == false)
                {
                    cancellationToken.Cancel();
                    waitEvent.Set();
                    updateTask.Wait();
                }

                waitEvent.Dispose();
                updateTask.Dispose();
                cancellationToken.Dispose();
            }
        }
    }
}