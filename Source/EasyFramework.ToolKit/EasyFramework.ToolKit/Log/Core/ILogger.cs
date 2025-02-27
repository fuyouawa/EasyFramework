using System;
using System.Diagnostics;

namespace EasyFramework.ToolKit
{

    public interface ILogger
    {
        public void Debug(string message);
        public void Info(string message);
        public void Warn(string message);
        public void Error(string message);
        public void Error(Exception exception, string message = null);
        public void Fatal(string message);
        public void Fatal(Exception exception, string message = null);
    }
}
