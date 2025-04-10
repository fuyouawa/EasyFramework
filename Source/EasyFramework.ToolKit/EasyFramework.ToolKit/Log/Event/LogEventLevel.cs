using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public enum LogEventLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public static class LogEventLevelExtension
    {
        public static LogType ToUnityLogType(this LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Debug:
                case LogEventLevel.Info:
                    return LogType.Log;
                case LogEventLevel.Warn:
                    return LogType.Warning;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return LogType.Error;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}
