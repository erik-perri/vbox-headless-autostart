using Microsoft.Extensions.Logging;

namespace TrayApp.Logging
{
    public static class LogLevelConfigurationManager
    {
        public static readonly LogLevel DefaultLevel = LogLevel.Information;

        public static void SetLogLevel(LogLevel level)
        {
            NLog.GlobalDiagnosticsContext.Set("LogLevel", level.ToString());
            NLog.LogManager.ReconfigExistingLoggers();
        }
    }
}