using TMPro;
using UnityEngine;
using EasyToolKit.Core;

namespace EasyToolKit.UIKit
{
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextStyleApplier : MonoBehaviour
    {
        [SerializeField] private string _styleName;
        [SerializeField] private bool _ignoreFontAsset;
        [SerializeField] private bool _ignoreFontMaterial;
        [SerializeField] private bool _ignoreFontSize;
        [SerializeField] private bool _ignoreFontColor;

        private bool _isInitialized;

        public string StyleName
        {
            get => _styleName;
            set => _styleName = value;
        }

        private TextMeshProUGUI _text;
        
        public TextStyle GetStyle()
        {
            return TextStyleLibrary.Instance.GetStyleByName(_styleName);
        }

        void OnEnable()
        {
            if (!_isInitialized)
            {
                if (_styleName.IsNullOrEmpty())
                {
                    _styleName = TextStyleLibrary.Instance.GetDefaultStyle()?.Name;
                }
                _isInitialized = true;
            }
            ApplyStyle();
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                ApplyStyle();
            }
        }

        public void ApplyStyle()
        {
            _text ??= GetComponent<TextMeshProUGUI>();

            var style = GetStyle();

            if (style != null)
            {
                if (!_ignoreFontAsset)
                {
                    _text.font = style.FontAsset;
                }

                if (!_ignoreFontMaterial)
                {
                    _text.fontSharedMaterial = style.FontMaterial;
                }

                if (!_ignoreFontSize)
                {
                    _text.fontSize = style.FontSize;
                }

                if (!_ignoreFontColor)
                {
                    _text.color = style.FontColor;
                }
            }
        }
    }
}
