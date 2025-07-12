using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using EasyToolKit.Core;

namespace EasyToolKit.GameConsole
{
    public class GameConsoleLogWindow : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _pageMaxNumber = 10;

        [Header("Binding")]
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
        private GameConsoleManager _manager;

        private List<GameConsoleLogItem> _items = new List<GameConsoleLogItem>();
        private int _nextVailedItemIndex = 0;

        private int _currentPageNum = 0;
        private int _pageNum = 0;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _manager = GameConsoleManager.Instance;
            _clearButton.onClick.AddListener(() => _manager.ClearLogs());

            _collapseButton.onClick.AddListener(() => _manager.HideLogWindow());
            _closeButton.onClick.AddListener(() => _manager.Close());

            _sendCommandButton.onClick.AddListener(SendCommand);

            _manager.InfoLogCount.OnValueChanged += val => _infoCountText.text = val.ToString();
            _manager.WarnLogCount.OnValueChanged += val => _warnCountText.text = val.ToString();
            _manager.ErrorLogCount.OnValueChanged += val => _errorCountText.text = val.ToString();

            _previousPageButton.onClick.AddListener(() => PrevPage());
            _nextPageButton.onClick.AddListener(() => NextPage());

            GameConsoleManager.Instance.OnPushLog += OnPushLog;
            GameConsoleManager.Instance.OnClearLogs += OnClearLogs;
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
                var item = Instantiate(GameConsoleManager.Instance.Config.LogItemPrefab, _logsContainer, false);
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
            _pageNum = _manager.LogItemDataList.Count / (_pageMaxNumber + 1);   // 当数量和最大数量相同时，也算做当前页；超过1个才算新增一页
            _currentPageNum = Mathf.Min(_pageNum, _currentPageNum);

            _pageNumberText.text = $"{_currentPageNum + 1}/{_pageNum + 1}";
        }

        private void RefreshPage()
        {
            ClearPage();
            var idx = _currentPageNum * _pageMaxNumber;
            var count = Mathf.Min(_manager.LogItemDataList.Count - idx, _pageMaxNumber);

            for (int i = 0; i < count; i++)
            {
                var data = _manager.LogItemDataList[idx + i];
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
                GameConsoleManager.Instance.LogError(errmsg);
                Debug.LogError(errmsg);
                return;
            }

            var cmd = match.Groups[1].Value;
            var c = GameConsoleCommandsManager.Instance.GetCommand(cmd);
            if (c == null)
            {
                var errmsg = $"Unknown command: {cmd}";
                GameConsoleManager.Instance.Log(GameConsoleLogType.Error, errmsg);
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
                GameConsoleManager.Instance.Log(GameConsoleLogType.Error, errmsg);
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
