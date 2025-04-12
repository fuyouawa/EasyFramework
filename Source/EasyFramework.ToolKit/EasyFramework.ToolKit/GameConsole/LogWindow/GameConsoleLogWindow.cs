using System;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFramework.ToolKit
{
    public class GameConsoleLogWindow : MonoBehaviour
    {
        [Title("Bindings")] [SerializeField] private Button _btnClear;
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnCollapse;
        [SerializeField] private Button _btnSendCommand;

        [SerializeField] private InputField _inputSearch;
        [SerializeField] private InputField _inputCommand;

        [SerializeField] private Text _textInfoCount;
        [SerializeField] private Text _textWarnCount;
        [SerializeField] private Text _textErrorCount;

        [SerializeField] private RectTransform _logsContainer;

        private CanvasGroup _canvasGroup;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            var console = GameConsole.Instance;
            _btnClear.onClick.AddListener(() => console.ClearLogs());

            _btnCollapse.onClick.AddListener(() => console.HideLogWindow());
            _btnClose.onClick.AddListener(() => console.Close());

            _btnSendCommand.onClick.AddListener(SendCommand);

            console.InfoLogCount.OnValueChanged.Register(val => _textInfoCount.text = val.ToString());
            console.WarnLogCount.OnValueChanged.Register(val => _textWarnCount.text = val.ToString());
            console.ErrorLogCount.OnValueChanged.Register(val => _textErrorCount.text = val.ToString());

            GameConsole.Instance.OnPushLog += OnPushLog;
            GameConsole.Instance.OnClearLogs += OnClearLogs;
        }

        void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            Clear();
            foreach (var data in GameConsole.Instance.LogItemDataStack)
            {
                SpawnLogItem(data);
            }
        }

        public void Clear()
        {
            var items = _logsContainer.GetComponentsInChildren<GameConsoleLogItem>();
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        private void SendCommand()
        {
            if (_inputCommand.text.IsNullOrWhiteSpace())
                return;

            var originCmd = _inputCommand.text.Trim();

            var match = Regex.Match(originCmd, @"^(\w+)(?:\s+(.+))?$");
            if (!match.Success)
            {
                var errmsg = "Command format error, must be: <command> [argument]";
                GameConsole.Instance.LogError(errmsg);
                Debug.LogError(errmsg);
                return;
            }

            var cmd = match.Groups[1].Value;
            var c = GameConsoleCommandsManager.Instance.GetCommand(cmd);
            if (c == null)
            {
                var errmsg = $"Unknown command: {cmd}";
                GameConsole.Instance.Log(GameConsole.LogType.Error, errmsg);
                Debug.LogError(errmsg);
                return;
            }
            
            // GameConsole.Instance.LogInfo($"Input Command: {cmd}");

            try
            {
                c.Send(match.Groups[2].Success ? match.Groups[2].Value : null);
            }
            catch (Exception e)
            {
                var errmsg = $"Send command failed: {e.Message}";
                GameConsole.Instance.Log(GameConsole.LogType.Error, errmsg);
                Debug.LogError(errmsg);
                return;
            }
        }

        private void OnPushLog(GameConsoleLogItemData data)
        {
            SpawnLogItem(data);
        }

        private void OnClearLogs()
        {
            Clear();
        }

        private void SpawnLogItem(GameConsoleLogItemData data)
        {
            var item = Instantiate(GameConsoleSettings.Instance.LogItemPrefab, _logsContainer, false);
            item.Set(data);
        }
    }
}
