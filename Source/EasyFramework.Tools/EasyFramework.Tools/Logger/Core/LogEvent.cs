using System.Diagnostics;
using System;

namespace EasyFramework.Tools
{
    public class LogEvent
    {
        public DateTime Now;
        public LogEventLevel Level;
        public Exception Exception;
        public string Message;
        public StackTrace DebugStackTrace;
    }
}
