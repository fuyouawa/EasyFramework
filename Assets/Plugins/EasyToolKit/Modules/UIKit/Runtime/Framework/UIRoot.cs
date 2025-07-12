using System;
using UnityEngine;
using UnityEngine.UI;

namespace EasyToolKit.UIKit
{
    public class UIRoot : MonoBehaviour
    {
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasScaler _canvasScaler;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;

        [SerializeField] private RectTransform _backgroundLevel;
        [SerializeField] private RectTransform _commonLevel;
        [SerializeField] private RectTransform _popUILevel;

        private RectTransform _lowestLevel;
        private RectTransform _topestLevel;

        public Camera UICamera => _uiCamera;
        public Canvas Canvas => _canvas;
        public CanvasScaler CanvasScaler => _canvasScaler;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;

        public RectTransform BackgroundLevel => _backgroundLevel;
        public RectTransform CommonLevel => _commonLevel;
        public RectTransform PopUILevel => _popUILevel;
        public RectTransform LowestLevel => _lowestLevel;
        public RectTransform TopestLevel => _topestLevel;

        private void Awake()
        {
            // Create lowest level container
            _lowestLevel = CreateLevelContainer("__Lowest");
            _lowestLevel.SetSiblingIndex(0);

            // Create topest level container
            _topestLevel = CreateLevelContainer("__Topest");
            _topestLevel.SetAsLastSibling();
        }

        private RectTransform CreateLevelContainer(string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.SetParent(transform);
            
            ResetRectTransform(rectTransform);
            
            return rectTransform;
        }

        public void SetPanelLevel(IPanel panel, UILevel level)
        {
            switch (level)
            {
                case UILevel.Lowest:
                    panel.Transform.SetParent(_lowestLevel);
                    break;
                case UILevel.Background:
                    panel.Transform.SetParent(_backgroundLevel);
                    break;
                case UILevel.Common:
                    panel.Transform.SetParent(_commonLevel);
                    break;
                case UILevel.PopUI:
                    panel.Transform.SetParent(_popUILevel);
                    break;
                case UILevel.Topest:
                    panel.Transform.SetParent(_topestLevel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
            ResetRectTransform(panel.Transform as RectTransform);
        }

        /// <summary>
        /// 将RectTransform重置为全屏UI元素的默认值
        /// </summary>
        private void ResetRectTransform(RectTransform rectTransform)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.pivot = Vector2.one * 0.5f;
        }
    }
}
