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
        private readonly ILocatorService locatorService;
        private readonly MachineStore machineStore;

        private Task updateTask;
        private readonly MonitoredMachineFilter machineFilter;

        public MachineStoreUpdater(
            ILogger<MachineStoreUpdater> logger,
            ILocatorService locatorService,
            MachineStore machineStore,
            MonitoredMachineFilter machineFilter,
            ConfigurationStore configurationStore
        )
        {
            logger.LogTrace(".ctor");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.locatorService = locatorService ?? throw new ArgumentNullException(nameof(locatorService));
            this.machineStore = machineStore ?? throw new ArgumentNullException(nameof(machineStore));
            this.machineFilter = machineFilter ?? throw new ArgumentNullException(nameof(machineFilter));

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
                    machineStore.SetMachines(locatorService.LocateMachines(machineFilter, true));

                    waitEvent.WaitOne(1000);
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