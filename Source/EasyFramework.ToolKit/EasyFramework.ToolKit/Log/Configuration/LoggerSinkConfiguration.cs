using System;

namespace EasyFramework.ToolKit
{
    public class LoggerSinkConfiguration
    {
        private readonly LoggerConfiguration _loggerConfiguration;
        private readonly Action<ILogEventSink> _addSink;

        public LoggerSinkConfiguration(LoggerConfiguration loggerConfiguration, Action<ILogEventSink> addSink)
        {
            _loggerConfiguration = loggerConfiguration;
            _addSink = addSink;
        }


        public LoggerConfiguration Sink(ILogEventSink logEventSink)
        {
            if (logEventSink == null)
            {
                throw new ArgumentNullException("logEventSink cant be null!");
            }

            _addSink(logEventSink);
            return _loggerConfiguration;
        }
    }
}
