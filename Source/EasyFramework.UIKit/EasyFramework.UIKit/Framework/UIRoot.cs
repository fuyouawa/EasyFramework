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

        public Camera UICamera => _uiCamera;
        public Canvas Canvas => _canvas;
        public CanvasScaler CanvasScaler => _canvasScaler;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;

        public RectTransform LevelBackground => _levelBackground;
        public RectTransform LevelCommon => _levelCommon;
        public RectTransform LevelPopUi => _levelPopUI;

        public void SetPanelLevel(IPanel panel, UILevel level)
        {
            switch (level)
            {
                case UILevel.Lowest:
                    throw new NotImplementedException();
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
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
            FixPanelRectTransform(panel.Transform as RectTransform);
        }

        private void FixPanelRectTransform(RectTransform rectTransform)
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
