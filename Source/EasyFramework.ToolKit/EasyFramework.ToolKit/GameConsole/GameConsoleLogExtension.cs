using System;
using System.Diagnostics;

namespace EasyFramework.ToolKit
{
    public class GameConsoleLogEventSink : ILogEventSink
    {
        public void Emit(LogEvent e)
        {
            GameConsole.LogType logType;
            switch (e.Level)
            {
                case LogEventLevel.Debug:
                case LogEventLevel.Info:
                    logType = GameConsole.LogType.Info;
                    break;
                case LogEventLevel.Warn:
                    logType = GameConsole.LogType.Warn;
                    break;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    logType = GameConsole.LogType.Error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameConsole.Instance.Log(logType, e.Message);
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
