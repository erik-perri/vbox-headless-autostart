using Microsoft.Extensions.Logging;
using System;
using System.ServiceProcess;

namespace ServiceApp
{
    public partial class VBoxHeadlessAutoStart : ServiceBase
    {
        private readonly ILogger<VBoxHeadlessAutoStart> logger;

        public VBoxHeadlessAutoStart(ILogger<VBoxHeadlessAutoStart> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger.LogTrace("Service start requested");
        }

        protected override void OnStop()
        {
            logger.LogTrace("Service stop requested");
        }
    }
}