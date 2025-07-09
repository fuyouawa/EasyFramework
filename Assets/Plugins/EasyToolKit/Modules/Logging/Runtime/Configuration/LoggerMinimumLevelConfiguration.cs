using System;

namespace EasyToolKit.Logging
{
    public class LoggerMinimumLevelConfiguration
    {
        private readonly LoggerConfiguration _loggerConfiguration;
        private readonly Action<LogEventLevel> _setMinimum;

        public LoggerMinimumLevelConfiguration(LoggerConfiguration loggerConfiguration, Action<LogEventLevel> setMinimum)
        {
            _loggerConfiguration = loggerConfiguration;
            _setMinimum = setMinimum;
        }

        public LoggerConfiguration Is(LogEventLevel minimumLevel)
        {
            _setMinimum(minimumLevel);
            return _loggerConfiguration;
        }

        public LoggerConfiguration Debug() => Is(LogEventLevel.Debug);
        public LoggerConfiguration Info() => Is(LogEventLevel.Info);
        public LoggerConfiguration Warn() => Is(LogEventLevel.Warn);
        public LoggerConfiguration Error() => Is(LogEventLevel.Error);
        public LoggerConfiguration Fatal() => Is(LogEventLevel.Fatal);
    }
}
