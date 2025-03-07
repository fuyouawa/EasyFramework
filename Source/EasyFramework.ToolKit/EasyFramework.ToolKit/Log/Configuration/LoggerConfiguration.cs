using System;
using System.Collections.Generic;

namespace EasyFramework.ToolKit
{
    public class LoggerConfiguration
    {
        private List<ILogEventSink> _sinks = new List<ILogEventSink>();
        private bool _loggerCreated = false;
        private LogEventLevel _minimumLevel = LogEventLevel.Info;

        public LoggerSinkConfiguration WriteTo { get; }

        public LoggerMinimumLevelConfiguration MinimumLevel { get; }


        public LoggerConfiguration()
        {
            WriteTo = new LoggerSinkConfiguration(this, sink => _sinks.Add(sink));
            MinimumLevel = new LoggerMinimumLevelConfiguration(this, level => _minimumLevel = level);
        }

        public ILogger CreateLogger()
        {
            if (_loggerCreated) throw new InvalidOperationException("CreateLogger() was previously called and can only be called once.");
            _loggerCreated = true;

            var sink = new AggregateLogEventSink(_sinks);
            return new Logger(_minimumLevel, sink);
        }
    }
}
