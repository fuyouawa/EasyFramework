using System;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFramework.UIKit
{
    public class UIRoot : MonoBehaviour
    {
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasScaler _canvasScaler;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;

        [SerializeField] private RectTransform _levelBackground;
        [SerializeField] private RectTransform _levelCommon;
        [SerializeField] private RectTransform _levelPopUI;

        private RectTransform _levelLowest;
        private RectTransform _levelTopest;

        public Camera UICamera => _uiCamera;
        public Canvas Canvas => _canvas;
        public CanvasScaler CanvasScaler => _canvasScaler;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;

        public RectTransform LevelBackground => _levelBackground;
        public RectTransform LevelCommon => _levelCommon;
        public RectTransform LevelPopUi => _levelPopUI;
        public RectTransform LevelLowest => _levelLowest;
        public RectTransform LevelTopest => _levelTopest;

        private void Awake()
        {
            // Create lowest level container
            _levelLowest = CreateLevelContainer("__Lowest");
            _levelLowest.SetSiblingIndex(0);

            // Create topest level container
            _levelTopest = CreateLevelContainer("__Topest");
            _levelTopest.SetAsLastSibling();
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
                    panel.Transform.SetParent(_levelLowest);
                    break;
                case UILevel.Background:
                    panel.Transform.SetParent(_levelBackground);
                    break;
                case UILevel.Common:
                    panel.Transform.SetParent(_levelCommon);
                    break;
                case UILevel.PopUI:
                    panel.Transform.SetParent(_levelPopUI);
                    break;
                case UILevel.Topest:
                    panel.Transform.SetParent(_levelTopest);
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
