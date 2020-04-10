using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TrayApp.Helpers
{
    public static class ProcessExtensions
    {
        public static ProcessOutput GetOuput(this Process process, int timeoutInMs = -1, int timeoutExitCode = -35)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            var outputData = new List<string>();
            var errorData = new List<string>();

            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                var data = e.Data?.Trim();
                if (!string.IsNullOrWhiteSpace(data))
                {
                    outputData.Add(data);
                }
            });

            process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                var data = e.Data?.Trim();
                if (!string.IsNullOrWhiteSpace(data))
                {
                    errorData.Add(data);
                }
            });

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var exitCode = timeoutExitCode;
            if (process.WaitForExit(timeoutInMs))
            {
                exitCode = process.ExitCode;
            }

            return new ProcessOutput(
                exitCode,
                new ReadOnlyCollection<string>(outputData.ToArray()),
                new ReadOnlyCollection<string>(errorData.ToArray())
            );
        }
    }
}