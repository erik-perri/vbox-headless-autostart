using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrayApp.State
{
    public class MachineStateUpdater : IDisposable
    {
        private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);
        private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private readonly List<string> fastUpdateReasons = new List<string>();

        private readonly AppState appState;

        private Task updateTask;

        public MachineStateUpdater(AppState appState)
        {
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));

            appState.OnConfigurationChange += (object _, EventArgs __) => RequestUpdate();
        }

        public void RequestFastUpdates(string reason)
        {
            fastUpdateReasons.Add(reason);
            RequestUpdate();
        }

        public void RemoveFastUpdateRequest(string reason) => fastUpdateReasons.Remove(reason);

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
                    appState.UpdateMachines();

                    waitEvent.WaitOne(fastUpdateReasons.Count > 0 ? 250 : 5000);
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