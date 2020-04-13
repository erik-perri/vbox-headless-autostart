using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TrayApp.Helpers
{
    public static class ProcessExtensions
    {
        public static ProcessOutput GetOuput(
            this Process process,
            int timeoutInMs = -1,
            int timeoutExitCode = -35,
            ILogger logger = null
        )
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

            if (!process.Start())
            {
                if (logger != null)
                {
                    var logOutput = new
                    {
                        process.StartInfo.FileName,
                        process.StartInfo.Arguments,
                        LastError = Marshal.GetLastWin32Error(),
                    };
                    logger.LogTrace($"Failed to execute process {logOutput}");
                }

                return new ProcessOutput(
                    -1,
                    null,
                    null
                );
            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var exitCode = timeoutExitCode;
            if (process.WaitForExit(timeoutInMs))
            {
                exitCode = process.ExitCode;
            }

            if (logger != null)
            {
                var logOutput = new
                {
                    process.StartInfo.FileName,
                    process.StartInfo.Arguments,
                    ExitCode = exitCode,
                    OutputData = string.Join("\r\n", outputData.ToArray()),
                    ErrorData = string.Join("\r\n", errorData.ToArray()),
                };
                logger.LogTrace($"Process executed {logOutput}");
            }

            return new ProcessOutput(
                exitCode,
                new ReadOnlyCollection<string>(outputData.ToArray()),
                new ReadOnlyCollection<string>(errorData.ToArray())
            );
        }
    }
}