using TMPro;
using UnityEngine;

namespace EasyFramework.UIKit
{
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextStyleApplier : MonoBehaviour
    {
        [SerializeField] private string _styleName;

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
                _text.font = style.FontAsset;
                _text.fontSharedMaterial = style.FontMaterial;
                _text.fontSize = style.FontSize;
                _text.color = style.FontColor;
            }
        }
    }
}
