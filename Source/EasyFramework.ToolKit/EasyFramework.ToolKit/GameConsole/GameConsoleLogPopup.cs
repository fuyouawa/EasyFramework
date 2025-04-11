using UnityEngine;
using UnityEngine.UI;

namespace EasyFramework.ToolKit
{
    public class GameConsoleLogPopup : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        [SerializeField] private Text _textInfoCount;
        [SerializeField] private Text _textWarnCount;
        [SerializeField] private Text _textErrorCount;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            var console = GameConsole.Instance;
            console.InfoLogCount.OnValueChanged.Register(val => _textInfoCount.text = val.ToString());
            console.WarnLogCount.OnValueChanged.Register(val => _textWarnCount.text = val.ToString());
            console.ErrorLogCount.OnValueChanged.Register(val => _textErrorCount.text = val.ToString());
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
    }
}
