﻿using CommonLib.VirtualMachine;
using CommonLib.VirtualMachine.VirtualBox;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TrayApp.Helpers;

namespace TrayApp.VirtualMachine.VirtualBox
{
    public class MachineLocator : IMachineLocator
    {
        private readonly ILogger<MachineLocator> logger;
        private readonly MetadataReader metadataReader;

        public MachineLocator(ILogger<MachineLocator> logger, MetadataReader metadataReader)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.metadataReader = metadataReader ?? throw new ArgumentNullException(nameof(metadataReader));
        }

        public IMachine[] LocateMachines(IMachineFilter filter, bool loadMetadata)
        {
            var machines = GetMachinesFromVBoxManage(filter);

            if (loadMetadata)
            {
                UpdateMetadataInMachinesAsync(machines).Wait();
            }

            return machines;
        }

        public IMachine[] LocateMachines(bool loadMetadata)
        {
            return LocateMachines(null, loadMetadata);
        }

        private async Task UpdateMetadataInMachinesAsync(IMachine[] machines)
        {
            var tasks = new List<Task>();

            foreach (var machine in machines)
            {
                tasks.Add(Task.Factory.StartNew(
                    () => machine.Metadata = metadataReader.ReadMetadata(machine) ?? new MachineMetadata(),
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

        private IMachine[] GetMachinesFromVBoxManage(IMachineFilter filter)
        {
            using var process = new VBoxManageProcess($"list vms");

            var output = process.GetOuput();
            var machines = new List<IMachine>();

            foreach (var line in output.OutputData)
            {
                var match = Regex.Match(
                    line,
                    @"^""(.*)"" \{([a-fA-F0-9]{8}\-[a-fA-F0-9]{4}\-[a-fA-F0-9]{4}\-[a-fA-F0-9]{4}\-[a-fA-F0-9]{12})\}$"
                );
                if (!match.Success)
                {
                    logger.LogWarning($"Failed to parse VBoxManage output {new { Line = line }}");
                    continue;
                }

                var uuid = match.Groups[2].Value.Trim();
                var name = match.Groups[1].Value;

                if (filter?.IncludeMachine(uuid) == false)
                {
                    continue;
                }

                machines.Add(new Machine(uuid, name));
            }

            return machines.ToArray();
        }
    }
}