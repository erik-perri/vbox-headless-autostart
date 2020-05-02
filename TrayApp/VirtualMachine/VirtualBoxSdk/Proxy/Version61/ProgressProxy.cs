using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using VirtualBox61;

namespace TrayApp.VirtualMachine.VirtualBoxSdk.Proxy.Version61
{
    public class ProgressProxy : IProgressProxy
    {
        private readonly ILogger logger;
        private readonly IProgress progress;

        public ProgressProxy(ILogger logger, IProgress progress)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.progress = progress ?? throw new ArgumentNullException(nameof(progress));
        }

        public void WaitForCompletion(int timeout)
        {
            try
            {
                progress.WaitForCompletion(timeout);
            }
            catch (COMException e)
            {
                logger.LogError(e, $"COM exception caught in WaitForCompletion({timeout})");
            }
        }

        public int ResultCode => progress.ResultCode;

        public bool CheckForSuccess()
        {
            WaitForCompletion(-1);

            return ResultCode == 0;
        }
    }
}