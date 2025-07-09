using System;

namespace EasyToolKit.Logging
{
    public static class Log
    {
        public static ILogger Logger { get; set; }

        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Warn(string message)
        {
            Logger.Warn(message);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
        }

        public static void Error(Exception exception, string message = null)
        {
            Logger.Error(exception, message);
        }

        public static void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public static void Fatal(Exception exception, string message = null)
        {
            Logger.Fatal(exception, message);
        }
    }
}
