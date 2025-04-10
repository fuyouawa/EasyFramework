using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFramework.ToolKit
{
    public class GameConsoleLogWindow : MonoBehaviour
    {
        [Title("Bindings")]
        [SerializeField] private Button _btnClear;
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnCollapse;

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
