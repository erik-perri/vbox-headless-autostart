using Microsoft.Extensions.Logging;

namespace TrayApp.Logging
{
    public static class LogLevelConfigurationManager
    {
        public static readonly LogLevel DefaultLevel = LogLevel.Information;

        public static void SetLogLevel(LogLevel level)
        {
            NLog.GlobalDiagnosticsContext.Set("LogLevel", ConvertLogLevelToNLog(level));
            NLog.LogManager.ReconfigExistingLoggers();
        }

        public static string ConvertLogLevelToNLog(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => "Trace",
                LogLevel.Debug => "Debug",
                LogLevel.Information => "Info",
                LogLevel.Warning => "Warn",
                LogLevel.Error => "Error",
                LogLevel.Critical => "Fatal",
                LogLevel.None => "None",
                _ => null,
            };
        }
    }
}