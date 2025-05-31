using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using EasyFramework.Core;

namespace EasyFramework.ToolKit
{
    public class GameConsoleLogWindow : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField, DisableInPlayMode] private int _pageMaxNumber = 10;

        [Title("Binding")]
        [SerializeField] private Button _clearButton;
        [SerializeField] private InputField _searchInputField;
        [SerializeField] private Text _infoCountText;
        [SerializeField] private Text _warnCountText;
        [SerializeField] private Text _errorCountText;
        [SerializeField] private Button _collapseButton;
        [SerializeField] private Button _closeButton;

        [SerializeField] private InputField _commandInputField;
        [SerializeField] private Button _sendCommandButton;

        [SerializeField] private Button _previousPageButton;
        [SerializeField] private Text _pageNumberText;
        [SerializeField] private Button _nextPageButton;

        [SerializeField] private RectTransform _logsContainer;

        private CanvasGroup _canvasGroup;
        private GameConsole _console;

        private List<GameConsoleLogItem> _items = new List<GameConsoleLogItem>();
        private int _nextVailedItemIndex = 0;

        private int _currentPageNum = 0;
        private int _pageNum = 0;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _console = GameConsole.Instance;
            _clearButton.onClick.AddListener(() => _console.ClearLogs());

            _collapseButton.onClick.AddListener(() => _console.HideLogWindow());
            _closeButton.onClick.AddListener(() => _console.Close());

            _sendCommandButton.onClick.AddListener(SendCommand);

            _console.InfoLogCount.OnValueChanged.Register(val => _infoCountText.text = val.ToString());
            _console.WarnLogCount.OnValueChanged.Register(val => _warnCountText.text = val.ToString());
            _console.ErrorLogCount.OnValueChanged.Register(val => _errorCountText.text = val.ToString());

            _previousPageButton.onClick.AddListener(() => PrevPage());
            _nextPageButton.onClick.AddListener(() => NextPage());

            GameConsole.Instance.OnPushLog += OnPushLog;
            GameConsole.Instance.OnClearLogs += OnClearLogs;
        }

        void Start()
        {
            foreach (var item in _logsContainer.GetComponentsInChildren<GameConsoleLogItem>())
            {
                Destroy(item.gameObject);
            }

            Rebuild();
        }

        private void Rebuild()
        {
            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }

            _items.Clear();
            _nextVailedItemIndex = 0;

            for (int i = 0; i < _pageMaxNumber; i++)
            {
                var item = Instantiate(GameConsole.Instance.Config.LogItemPrefab, _logsContainer, false);
                item.gameObject.SetActive(false);
                _items.Add(item);
            }

            ClearPage();
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

        public bool PrevPage()
        {
            if (_currentPageNum > 0)
            {
                _currentPageNum--;
                RefreshPageNum();
                RefreshPage();
                return true;
            }

            return false;
        }

        public bool NextPage()
        {
            if (_currentPageNum < _pageNum)
            {
                _currentPageNum++;
                RefreshPageNum();
                RefreshPage();
                return true;
            }

            return false;
        }

        private void ClearPage()
        {
            foreach (var item in _items)
            {
                item.gameObject.SetActive(false);
                item.Clear();
            }

            _nextVailedItemIndex = 0;
        }

        private void RefreshPageNum()
        {
            _pageNum = _console.LogItemDataList.Count / (_pageMaxNumber + 1);   // 当数量和最大数量相同时，也算做当前页；超过1个才算新增一页
            _currentPageNum = Mathf.Min(_pageNum, _currentPageNum);

            _pageNumberText.text = $"{_currentPageNum + 1}/{_pageNum + 1}";
        }

        private void RefreshPage()
        {
            ClearPage();
            var idx = _currentPageNum * _pageMaxNumber;
            var count = Mathf.Min(_console.LogItemDataList.Count - idx, _pageMaxNumber);

            for (int i = 0; i < count; i++)
            {
                var data = _console.LogItemDataList[idx + i];
                AppendLog(data);
            }
        }

        private void SendCommand()
        {
            if (_commandInputField.text.IsNullOrWhiteSpace())
                return;

            var originCmd = _commandInputField.text.Trim();

            var match = Regex.Match(originCmd, @"^(\S+)(?:\s+(.+))?$");
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
            RefreshPageNum();
            AppendLog(data);
        }

        private bool AppendLog(GameConsoleLogItemData data)
        {
            if (_nextVailedItemIndex < _pageMaxNumber)
            {
                var item = _items[_nextVailedItemIndex++];
                item.gameObject.SetActive(true);
                item.Set(data);
                return true;
            }

            return false;
        }

        private void OnClearLogs()
        {
            ClearPage();
            RefreshPageNum();
        }
    }
}
