namespace CommonLib.Processes
{
    public class ProcessOutput : IProcessOutput
    {
        public ProcessOutput(int processId, int exitCode, string standardOutput, string standardError)
        {
            ProcessId = processId;
            ExitCode = exitCode;
            StandardOutput = standardOutput;
            StandardError = standardError;
        }

        public int ProcessId { get; }

        public int ExitCode { get; } = 259;

        public string StandardOutput { get; }

        public string StandardError { get; }
    }
}