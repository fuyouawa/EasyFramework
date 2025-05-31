using UnityEngine;
using UnityEngine.UI;

namespace EasyFramework.ToolKit
{
    public class GameConsoleLogPopup : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        [SerializeField] private Text _infoCountText;
        [SerializeField] private Text _warnCountText;
        [SerializeField] private Text _errorCountText;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            var console = GameConsole.Instance;
            console.InfoLogCount.OnValueChanged.Register(val => _infoCountText.text = val.ToString());
            console.WarnLogCount.OnValueChanged.Register(val => _warnCountText.text = val.ToString());
            console.ErrorLogCount.OnValueChanged.Register(val => _errorCountText.text = val.ToString());
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
