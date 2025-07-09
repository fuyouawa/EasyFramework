using System.Diagnostics;
using System;
using UnityEngine;

namespace EasyToolKit.Logging
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
