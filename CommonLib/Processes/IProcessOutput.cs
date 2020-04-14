using System.Linq;
using System.Text.RegularExpressions;

namespace CommonLib.Processes
{
    public interface IProcessOutput
    {
        int ProcessId { get; }

        int ExitCode { get; }

        string StandardOutput { get; }

        string StandardError { get; }

        string[] GetStandardOutputLines()
        {
            return Regex.Split(StandardOutput, "\r\n|\r|\n").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        }

        string[] GetStandardErrorLines()
        {
            return Regex.Split(StandardError, "\r\n|\r|\n").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        }
    }
}