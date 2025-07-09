using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace EasyToolKit.Logging
{
    internal class Logger : ILogger
    {
        public LogEventLevel MinimumLevel { get; }
        public ILogEventSink Sink { get; }

        public Logger(LogEventLevel minimumLevel, ILogEventSink sink)
        {
            MinimumLevel = minimumLevel;
            Sink = sink;
        }

        private bool IsEnableLevel(LogEventLevel level)
        {
            return level >= MinimumLevel;
        }

        private void Dispatch(LogEvent e)
        {
            Sink.Emit(e);
        }

        private void Write(LogEventLevel level, [CanBeNull] Exception exception, string message)
        {
            if (!IsEnableLevel(level))
                return;
            var e = new LogEvent()
            {
                Level = level,
                Exception = exception,
                Message = message,
                Now = DateTime.Now,
                DebugStackTrace = new StackTrace(true)
            };
            Dispatch(e);
        }

        public void Debug(string message)
        {
            Write(LogEventLevel.Debug, null, message);
        }

        public void Info(string message)
        {
            Write(LogEventLevel.Info, null, message);
        }

        public void Warn(string message)
        {
            Write(LogEventLevel.Warn, null, message);
        }

        public void Error(string message)
        {
            Write(LogEventLevel.Error, null, message);
        }

        public void Error(Exception exception, string message = null)
        {
            Write(LogEventLevel.Error, exception, message);
        }

        public void Fatal(string message)
        {
            Write(LogEventLevel.Fatal, null, message);
        }

        public void Fatal(Exception exception, string message = null)
        {
            Write(LogEventLevel.Fatal, exception, message);
        }
    }
}
