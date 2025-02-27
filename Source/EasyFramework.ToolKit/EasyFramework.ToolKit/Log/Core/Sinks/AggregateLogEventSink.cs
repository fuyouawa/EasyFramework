using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    internal class AggregateLogEventSink : ILogEventSink
    {
        readonly ILogEventSink[] _sinks;

        public AggregateLogEventSink(IEnumerable<ILogEventSink> sinks)
        {
            _sinks = sinks.ToArray();
        }

        public void Emit(LogEvent logEvent)
        {
            foreach (var sink in _sinks)
            {
                try
                {
                    sink.Emit(logEvent);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Caught exception while emitting to sink {sink}: {ex}");
                }
            }
        }
    }
}
