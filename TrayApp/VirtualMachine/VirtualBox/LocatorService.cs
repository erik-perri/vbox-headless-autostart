using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TrayApp.Extensions;

namespace TrayApp.VirtualMachine.VirtualBox
{
    public class LocatorService : ILocatorService
    {
        private readonly ILogger<LocatorService> logger;
        private readonly MetadataReader metadataReader;

        public LocatorService(ILogger<LocatorService> logger, MetadataReader metadataReader)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.metadataReader = metadataReader ?? throw new ArgumentNullException(nameof(metadataReader));
        }

        public IMachine[] LocateMachines(bool loadMetadata)
        {
            var machines = GetMachinesFromVBoxManage();

            if (loadMetadata)
            {
                UpdateMetadataInMachinesAsync(machines).Wait();
            }

            return machines;
        }

        private async Task UpdateMetadataInMachinesAsync(IMachine[] machines)
        {
            var tasks = new List<Task>();

            foreach (var machine in machines)
            {
                tasks.Add(Task.Factory.StartNew(
                    () => machine.Metadata = metadataReader.ReadMetadata(machine),
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskScheduler.Default
                ));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            foreach (var task in tasks)
            {
                task?.Dispose();
            }
        }

        private IMachine[] GetMachinesFromVBoxManage()
        {
            using var process = new VBoxManageProcess($"list vms");

            var output = process.GetOuput(100);
            var machines = new List<IMachine>();

            foreach (var line in output.OutputData)
            {
                var match = Regex.Match(
                    line,
                    @"^""(.*)"" \{([a-fA-F0-9]{8}\-[a-fA-F0-9]{4}\-[a-fA-F0-9]{4}\-[a-fA-F0-9]{4}\-[a-fA-F0-9]{12})\}$"
                );
                if (!match.Success)
                {
                    logger.LogWarning($"Failed to parse VBoxManage output, line: \"{line}\"");
                    continue;
                }

                machines.Add(new Machine(match.Groups[2].Value, match.Groups[1].Value));
            }

            return machines.ToArray();
        }
    }
}