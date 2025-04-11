using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    internal struct GameConsoleLogItemData
    {
        public DateTime Time { get; set; }
        public GameConsole.LogType LogType { get; set; }
        public string Message { get; set; }
        public StackTrace StackTrace { get; set; }
    }

    public class GameConsole : MonoSingleton<GameConsole>
    {
        public enum LogType
        {
            Info,
            Warn,
            Error
        }

        [SerializeField] private GameConsoleLogWindow _logWindow;
        [SerializeField] private GameConsoleLogPopup _logPopup;

        internal event Action<GameConsoleLogItemData> OnPushLog;
        internal event Action OnClearLogs;

        internal readonly Stack<GameConsoleLogItemData> LogItemDataStack = new Stack<GameConsoleLogItemData>();

        private readonly IBindableValue<int> _infoLogCount = new BindableValue<int>();
        private readonly IBindableValue<int> _warnLogCount = new BindableValue<int>();
        private readonly IBindableValue<int> _errorLogCount = new BindableValue<int>();

        public IReadonlyBindableValue<int> InfoLogCount => _infoLogCount;
        public IReadonlyBindableValue<int> WarnLogCount => _warnLogCount;
        public IReadonlyBindableValue<int> ErrorLogCount => _errorLogCount;

        public void ClearLogs()
        {
            LogItemDataStack.Clear();
            OnClearLogs?.Invoke();

            _infoLogCount.SetValue(0);
            _warnLogCount.SetValue(0);
            _errorLogCount.SetValue(0);
        }

        public void ShowLogWindow()
        {
            _logWindow.Show();
            _logPopup.Hide();
        }

        public void HideLogWindow()
        {
            _logWindow.Hide();
            _logPopup.Show();
        }

        public void Close()
        {
            Destroy(gameObject);
        }

        public void PushLog(LogType logType, string message, StackTrace stackTrace)
        {
            var data = new GameConsoleLogItemData()
            {
                Time = DateTime.Now,
                LogType = logType,
                Message = message,
                StackTrace = stackTrace
            };
            LogItemDataStack.Push(data);
            OnPushLog?.Invoke(data);
            switch (logType)
            {
                case LogType.Info:
                    _infoLogCount.SetValue(_infoLogCount.Value + 1);
                    break;
                case LogType.Warn:
                    _warnLogCount.SetValue(_warnLogCount.Value + 1);
                    break;
                case LogType.Error:
                    _errorLogCount.SetValue(_errorLogCount.Value + 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }
    }
}
