using System;
using UnityEngine;

namespace EasyFramework.LogEventSinks
{
    public class UnityConsoleLogEventSink : ILogEventSink
    {
        private readonly UnityEngine.ILogger _unityLogger;

        public UnityConsoleLogEventSink(UnityEngine.ILogger unityLogger)
        {
            _unityLogger = unityLogger;
        }

        public void Emit(LogEvent e)
        {
            LogType type;
            switch (e.Level)
            {
                case LogEventLevel.Debug:
                case LogEventLevel.Info:
                    type = LogType.Log;
                    break;
                case LogEventLevel.Warn:
                    type = LogType.Warning;
                    break;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    type = LogType.Error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (e.Exception != null)
            {
                _unityLogger.LogException(e.Exception);
            }
            else
            {
                _unityLogger.Log(type, e.Message);
            }
        }
    }
}
