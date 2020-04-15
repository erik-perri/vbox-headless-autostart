using System.Diagnostics;

namespace CommonLib.Processes
{
    public class ProcessOutputFactory : IProcessOutputFactory
    {
        public IProcessOutput CreateProcess(string fileName, string arguments)
        {
            return CreateProcess(fileName, arguments, true);
        }

        public static IProcessOutput CreateProcess(string fileName, string arguments, bool hideWindow)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = fileName,
                    Arguments = arguments,
                    CreateNoWindow = hideWindow,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };

            process.Start();

            var standardOutput = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return new ProcessOutput((uint)process.Id, process.ExitCode, standardOutput, standardError);
        }
    }
}