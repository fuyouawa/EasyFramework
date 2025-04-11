using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EasyFramework.ToolKit
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
                var sb = new StringBuilder(e.Message);
                var stackTrack = GetStackTrack();
                if (stackTrack.IsNotNullOrWhiteSpace())
                {
                    sb.AppendLine();
                    sb.AppendLine(stackTrack);
                    sb.Append("--------- Unity Stack Track ---------");
                }
                _unityLogger.Log(type, sb.ToString());
            }
        }

        private string GetStackTrack()
        {
            var stackTrace = new StackTrace(true);
            var index = GetFrameIndex(stackTrace);
            var frames = stackTrace.GetFrames();

            if (frames == null || index >= frames.Length)
                return null;

            var sb = new StringBuilder();
            for (int i = index; i < frames.Length; i++)
            {
                var frame = frames[i];
                var method = frame.GetMethod();
                sb.Append($"{method.DeclaringType.Namespace}.{method.DeclaringType.Name}:");
                sb.Append(method.Name);

                var fileName = frame.GetFileName();
                var lineNum = frame.GetFileLineNumber();

                sb.Append(" at (");
                sb.Append($"<a href=\"{fileName}\" line={lineNum}>");
                sb.Append($"{fileName}:{lineNum}");
                sb.AppendLine("</a>)");
            }
            return sb.ToString();
        }

        private int GetFrameIndex(StackTrace stackTrace)
        {
            var frames = stackTrace.GetFrames();
            if (frames == null)
                return 0;

            for (int i = 0; i < frames.Length; i++)
            {
                var frame = frames[i];
                var fileName = frame.GetFileName();
                if (fileName.IsNullOrEmpty())
                {
                    return i;
                }

                if (fileName.Contains("EasyFramework.ToolKit\\Log\\Core\\Log.cs"))
                {
                    return i + 1;
                }
            }

            return 0;
        }
    }
}
