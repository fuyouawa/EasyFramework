using System;
using System.Diagnostics;
using EasyToolKit.Logging;

namespace EasyToolKit.GameConsole
{
    public class GameConsoleLogEventSink : ILogEventSink
    {
        public void Emit(LogEvent e)
        {
            GameConsoleLogType logType;
            switch (e.Level)
            {
                case LogEventLevel.Debug:
                case LogEventLevel.Info:
                    logType = GameConsoleLogType.Info;
                    break;
                case LogEventLevel.Warn:
                    logType = GameConsoleLogType.Warn;
                    break;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    logType = GameConsoleLogType.Error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameConsoleManager.Instance.Log(logType, e.Message);
        }
    }


    public static class GameConsoleLogExtension
    {
        public static LoggerConfiguration GameConsole(this LoggerSinkConfiguration configuration)
        {
            return configuration.Sink(new GameConsoleLogEventSink());
        }
    }
}
