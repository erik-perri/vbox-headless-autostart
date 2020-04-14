using CommonLib.VirtualMachine;
using CommonLib.VirtualMachine.VirtualBox;
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

            var state = MachineState.Unknown;
            var lastAction = DateTime.MinValue;
            string sessionName = null;

            foreach (var line in output.OutputData)
            {
                var match = Regex.Match(line, @"^([A-Za-z0-9\s\(\)\-\.]+):?\s+(.*)$");
                if (!match.Success)
                {
                    continue;
                }

                var infoKey = match.Groups[1].Value.Trim();
                var infoValue = match.Groups[2].Value.Trim();

                switch (infoKey.ToUpperInvariant())
                {
                    case "STATE":
                        var parsed = ParseState(infoValue);
                        if (parsed != null)
                        {
                            state = parsed.Item1;
                            lastAction = parsed.Item2;
                        }
                        break;

                    case "SESSION NAME":
                        sessionName = infoValue.Trim();
                        break;
                }
            }

            if (state == MachineState.Unknown)
            {
                logger.LogWarning($"Failed to find state {new { machine.Uuid, machine.Name }}");

                return null;
            }

            return new MachineMetadata(state, lastAction, sessionName);
        }

        private Tuple<MachineState, DateTime> ParseState(string infoValue)
        {
            var stateMatch = Regex.Match(infoValue, @"^([A-Za-z\s]+)\s\(since ([0-9:\-\.T]+)\)$");
            if (!stateMatch.Success)
            {
                logger.LogWarning(
                    $"Failed to parse state {new { State = infoValue }}"
                );
                return null;
            }

            var foundState = stateMatch.Groups[1].Value.Trim();
            var foundDate = stateMatch.Groups[2].Value.Trim();

            var state = ParseVirtualBoxState(foundState);
            if (state == MachineState.Unknown)
            {
                logger.LogWarning($"Unknown state {new { State = foundState }}");
                return null;
            }

            if (!DateTime.TryParse(
                foundDate,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var lastAction
            ))
            {
                logger.LogWarning($"Failed to parse date {new { Date = foundDate }}");
            }

            return new Tuple<MachineState, DateTime>(state, lastAction);
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