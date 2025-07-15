using Serilog.Core;
using Serilog.Events;
using System;

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
                case LogEventLevel.Information:
                    logType = GameConsoleLogType.Info;
                    break;
                case LogEventLevel.Warning:
                    logType = GameConsoleLogType.Warn;
                    break;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    logType = GameConsoleLogType.Error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameConsoleManager.Instance.Log(logType, e.RenderMessage());
        }
    }

}
