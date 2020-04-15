namespace CommonLib.Processes
{
    public class ProcessOutput : IProcessOutput
    {
        public ProcessOutput(uint processId, int exitCode, string standardOutput, string standardError)
        {
            ProcessId = processId;
            ExitCode = exitCode;
            StandardOutput = standardOutput;
            StandardError = standardError;
        }

        public uint ProcessId { get; }

        public int ExitCode { get; } = 259;

        public string StandardOutput { get; }

        public string StandardError { get; }
    }
}