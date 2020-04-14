using CommonLib.Processes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.VirtualMachine.VirtualBox
{
    public class VBoxManageOutputFactory : IMachineLocator, IMachineController
    {
        private readonly ILogger<VBoxManageOutputFactory> logger;
        private readonly IProcessOutputFactory processFactory;

        public VBoxManageOutputFactory(
            ILogger<VBoxManageOutputFactory> logger,
            IProcessOutputFactory processFactory
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
        }

        public IMachine[] ListMachinesWithMetadata(IMachineFilter filter = null)
        {
            var machines = ListMachines(filter);

            UpdateMetadataInMachinesAsync(machines).Wait();

            return machines;
        }

        public IMachine[] ListMachines(IMachineFilter filter = null)
        {
            var output = processFactory.CreateProcess(LocateExecutable(), "list vms");
            if (output.ExitCode != 0)
            {
                throw new InvalidOperationException("Failed to obtain machine list");
            }

            var machines = ProcessMachineListOutput(output.GetStandardOutputLines());

            if (filter != null)
            {
                return machines.Where(m => filter.IncludeMachine(m.Uuid)).ToArray();
            }

            return machines.ToArray();
        }

        public IMachineMetadata GetMetadata(IMachine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            var output = processFactory.CreateProcess(LocateExecutable(), $"showvminfo {machine.Uuid}");
            if (output.ExitCode != 0)
            {
                throw new InvalidOperationException("Failed to obtain machine information");
            }

            var state = MachineState.Unknown;
            var lastAction = DateTime.MinValue;
            string sessionName = null;

            foreach (var line in output.GetStandardOutputLines())
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

        public bool StartMachine(IMachine machine, bool headless)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            var arguments = $"startvm {machine.Uuid}";
            if (headless)
            {
                arguments += " --type headless";
            }

            var output = processFactory.CreateProcess(LocateExecutable(), arguments);

            return output.ExitCode == 0;
        }

        public bool SaveState(IMachine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            var output = processFactory.CreateProcess(LocateExecutable(), $"controlvm {machine.Uuid} savestate");

            return output.ExitCode == 0;
        }

        public bool PowerOff(IMachine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            var output = processFactory.CreateProcess(LocateExecutable(), $"controlvm {machine.Uuid} poweroff");

            return output.ExitCode == 0;
        }

        public bool AcpiPowerOff(IMachine machine, int waitLimitInMilliseconds, Action onWaitAction)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            var output = processFactory.CreateProcess(LocateExecutable(), $"controlvm {machine.Uuid} acpipowerbutton");

            if (output.ExitCode != 0)
            {
                return false;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var waitTimeSpan = TimeSpan.FromMilliseconds(waitLimitInMilliseconds);
            while (GetMetadata(machine)?.State == MachineState.Running && stopwatch.Elapsed < waitTimeSpan)
            {
                onWaitAction();
                Thread.Sleep(500);
            }

            stopwatch.Stop();

            if (GetMetadata(machine)?.State != MachineState.PoweredOff)
            {
                logger.LogError($"Failed to power off {new { machine.Uuid, machine.Name }}");
                return false;
            }

            return true;
        }

        public bool Reset(IMachine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException(nameof(machine));
            }

            var output = processFactory.CreateProcess(LocateExecutable(), $"controlvm {machine.Uuid} reset");

            return output.ExitCode == 0;
        }

        private static string LocateExecutable()
        {
            var installPath = InstallPathLocator.FindInstallPath();
            if (installPath == null || !Directory.Exists(installPath))
            {
                throw new InvalidOperationException("VirtualBox path not found");
            }

            return $"{installPath}\\VBoxManage.exe";
        }

        private List<IMachine> ProcessMachineListOutput(string[] lines)
        {
            var machines = new List<IMachine>();

            foreach (var line in lines)
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

                machines.Add(new Machine(uuid, name));
            }

            return machines;
        }

        private Tuple<MachineState, DateTime> ParseState(string infoValue)
        {
            var stateMatch = Regex.Match(infoValue, @"^([A-Za-z\s]+)\s\(since ([0-9:\-\.T]+)\)$");
            if (!stateMatch.Success)
            {
                logger.LogWarning($"Failed to parse state {new { State = infoValue }}");
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
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var pascalState = textInfo.ToTitleCase(state).Replace(" ", "", StringComparison.Ordinal);

            if (Enum.TryParse(pascalState, out MachineState parsedState))
            {
                return parsedState;
            }

            return MachineState.Unknown;
        }

        private async Task UpdateMetadataInMachinesAsync(IMachine[] machines)
        {
            var tasks = new List<Task>();

            foreach (var machine in machines)
            {
                tasks.Add(Task.Factory.StartNew(
                    () => machine.Metadata = GetMetadata(machine) ?? new MachineMetadata(),
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
    }
}