using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrayApp.State
{
    public class MachineStateUpdater : IDisposable
    {
        private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);
        private readonly AppState appState;

        private CancellationTokenSource cancellationTokenSource;
        private Task updateTask;

        public MachineStateUpdater(AppState appState)
        {
            this.appState = appState ?? throw new ArgumentNullException(nameof(appState));

            appState.OnConfigurationChange += (object _, EventArgs __) => RequestUpdate();
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

            cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = cancellationTokenSource.Token;

            updateTask = Task.Factory.StartNew(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    appState.UpdateMachines();

                    waitEvent.WaitOne(250);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void StopMonitor()
        {
            if (updateTask?.IsCompleted == false)
            {
                cancellationTokenSource.Cancel();
                waitEvent.Set();
                updateTask.Wait();
            }
            updateTask?.Dispose();
            cancellationTokenSource?.Dispose();
            updateTask = null;
            cancellationTokenSource = null;
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
                    cancellationTokenSource.Cancel();
                    waitEvent.Set();
                    updateTask.Wait();
                }

                waitEvent.Dispose();
                updateTask?.Dispose();
                cancellationTokenSource?.Dispose();
            }
        }
    }
}