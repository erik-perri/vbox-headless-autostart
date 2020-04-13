using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TrayApp.Helpers;

namespace TrayApp.VirtualMachine.VirtualBox
{
    public class MetadataReader
    {
        private readonly ILogger<MetadataReader> logger;

        public MetadataReader(ILogger<MetadataReader> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IMachineMetadata ReadMetadata(IMachine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            using var process = new VBoxManageProcess($"showvminfo {machine.Uuid}");

            var output = process.GetOuput();
            var metadata = new MachineMetadata();

            foreach (var line in output.OutputData)
            {
                var match = Regex.Match(line, @"^([A-Za-z0-9\s\(\)\-\.]+):?\s+(.*)$");
                if (!match.Success)
                {
                    continue;
                }

                var infoKey = match.Groups[1].Value.Trim();

                if (!ParseInfoRow(infoKey, match.Groups[2].Value.Trim(), metadata))
                {
                    logger.LogWarning($"Failed to parse info line for machine { new { machine.Uuid, machine.Name }}");
                }
            }

            if (metadata.State == MachineState.Unknown)
            {
                logger.LogWarning($"Failed to find state {new { machine.Uuid, machine.Name }}");

                return null;
            }

            return metadata;
        }

        private bool ParseInfoRow(string infoName, string infoValue, MachineMetadata metadata)
        {
            switch (infoName.ToUpperInvariant())
            {
                case "STATE":
                    return ParseState(infoValue, metadata);

                case "SESSION NAME":
                    metadata.SessionName = infoValue.Trim();
                    return true;
            }

            return true;
        }

        private bool ParseState(string infoValue, MachineMetadata metadata)
        {
            var stateMatch = Regex.Match(infoValue, @"^([A-Za-z\s]+)\s\(since ([0-9:\-\.T]+)\)$");
            if (!stateMatch.Success)
            {
                logger.LogWarning(
                    $"Failed to parse state {new { State = infoValue }}"
                );
                return false;
            }

            var foundState = stateMatch.Groups[1].Value.Trim();
            var foundDate = stateMatch.Groups[2].Value.Trim();

            var state = ParseVirtualBoxState(foundState);
            if (state == MachineState.Unknown)
            {
                logger.LogWarning($"Unknown state {new { State = foundState }}");
                return false;
            }

            metadata.State = state;

            if (DateTime.TryParse(
                foundDate,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var lastAction
            ))
            {
                metadata.LastAction = lastAction;
            }
            else
            {
                logger.LogWarning($"Failed to parse date {new { Date = foundDate }}");
            }

            return true;
        }

        private static MachineState ParseVirtualBoxState(string state)
        {
            return state switch
            {
                "running" => MachineState.Running,
                "starting" => MachineState.Starting,
                "aborted" => MachineState.Aborted,
                "powered off" => MachineState.PoweredOff,
                "saved" => MachineState.StateSaved,
                "restoring" => MachineState.Restoring,
                "saving" => MachineState.Saving,
                _ => MachineState.Unknown,
            };
        }
    }
}