using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TrayApp.Extensions;

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

            foreach (var line in output.OutputData)
            {
                var match = Regex.Match(line, @"^([A-Za-z0-9\s\(\)\-\.]+):?\s+(.*)$");
                if (!match.Success)
                {
                    logger.LogWarning($"Failed to parse VBoxManage line for machine {machine.Uuid}, line: \"{line}\"");
                    continue;
                }

                var infoKey = match.Groups[1].Value.Trim();
                if (!infoKey.Equals("State", StringComparison.Ordinal))
                {
                    continue;
                }

                var infoValue = match.Groups[2].Value.Trim();
                var stateMatch = Regex.Match(infoValue, @"^([A-Za-z\s]+)\s\(since ([0-9:\-\.T]+)\)$");
                if (!stateMatch.Success)
                {
                    logger.LogWarning($"Failed to parse state output: \"{infoValue}\" for machine {machine.Uuid}");
                    break;
                }

                var foundState = stateMatch.Groups[1].Value.Trim();
                var foundDate = stateMatch.Groups[2].Value.Trim();

                var state = ParseVirtualBoxState(foundState);
                if (state == MachineState.Unknown)
                {
                    logger.LogWarning($"Unknown state found: \"{foundState}\" for machine {machine.Uuid}");
                    break;
                }

                var lastAction = DateTime.MinValue;
                if (!DateTime.TryParse(
                    foundDate,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out lastAction
                ))
                {
                    logger.LogWarning($"Failed to parse date: \"{foundDate}\" for machine {machine.Uuid}");
                }

                return new MachineMetadata(state, lastAction.ToLocalTime());
            }

            logger.LogWarning($"Failed to find state for machine {machine.Uuid}");

            return null;
        }

        private static MachineState ParseVirtualBoxState(string state)
        {
            return state switch
            {
                "running" => MachineState.Running,
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