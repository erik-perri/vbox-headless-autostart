using System.Collections.ObjectModel;

namespace TrayApp.Extensions
{
    public class ProcessOutput
    {
        public ProcessOutput(int exitCode, ReadOnlyCollection<string> outputData, ReadOnlyCollection<string> errorData)
        {
            ExitCode = exitCode;
            OutputData = outputData;
            ErrorData = errorData;
        }

        public int ExitCode { get; }

        public bool HasOutputData { get { return OutputData.Count > 0; } }

        public ReadOnlyCollection<string> OutputData { get; }

        public bool HasErrorData { get { return ErrorData.Count > 0; } }

        public ReadOnlyCollection<string> ErrorData { get; }
    }
}