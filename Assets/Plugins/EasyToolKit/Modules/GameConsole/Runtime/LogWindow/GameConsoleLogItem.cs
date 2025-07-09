using System;
using System.Diagnostics;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;
using UnityEngine.UI;

namespace EasyToolKit.GameConsole
{
    public class GameConsoleLogItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _messageText;
        [SerializeField] private Text _positionText;
        [SerializeField] private Button _copyButton;

        void Awake()
        {
            _copyButton.onClick.AddListener(() =>
            {
                GUIUtility.systemCopyBuffer = _messageText.text;
            });
        }

        public void Clear()
        {
            _icon.sprite = null;
            _messageText.text = null;
            _positionText.text = null;
        }

        internal void Set(GameConsoleLogItemData data)
        {
            switch (data.LogType)
            {
                case GameConsoleLogType.Info:
                    _icon.sprite = GameConsoleManager.Instance.Config.InfoLogIcon;
                    break;
                case GameConsoleLogType.Warn:
                    _icon.sprite = GameConsoleManager.Instance.Config.WarnLogIcon;
                    break;
                case GameConsoleLogType.Error:
                    _icon.sprite = GameConsoleManager.Instance.Config.ErrorLogIcon;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(data.LogType), data.LogType, null);
            }

            _messageText.text = $"[{data.Time:HH:mm:ss}] {data.Message}";
            var frame = GetFrame(data.StackTrace);
            if (frame == null)
            {
                _positionText.text = "";
            }
            else
            {
                var method = frame.GetMethod();
                _positionText.text = $"{method.DeclaringType.FullName}.{method.Name} at ({frame.GetFileName()}:{frame.GetFileLineNumber()})";
            }
        }

        private StackFrame GetFrame(StackTrace stackTrace)
        {
            var frames = stackTrace.GetFrames();
            if (frames == null)
                return null;

            const string logSign = "EasyFramework.ToolKit\\Log\\Core\\Log.cs";

            // 判断是否来自日志
            if (frames.Any(f => f.GetFileName()?.Contains(logSign) == true))
            {
                // 去除日志栈帧
                for (int i = 0; i < frames.Length; i++)
                {
                    var frame = frames[i];
                    var fileName = frame.GetFileName();

                    if (fileName.Contains(logSign))
                    {
                        return frames[i + 1];
                    }
                }
            }
            else
            {
                // 去除空白文件名
                for (int i = 0; i < frames.Length; i++)
                {
                    var frame = frames[i];
                    var fileName = frame.GetFileName();
                    var method = frame.GetMethod();
                    if (fileName.IsNotNullOrEmpty() && method.DeclaringType != typeof(GameConsoleManager))
                    {
                        return frame;
                    }
                }
            }

            return frames.FirstOrDefault();
        }
    }
}
