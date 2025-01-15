using EasyFramework;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace EasyGameFramework
{
    public class UiTextManager : SerializedMonoBehaviour
    {
        [SerializeField] private string _fontAssetPresetId;
        [SerializeField] private string _textPropertiesPresetId;

        private TextMeshProUGUI _text;

        private void Awake()
        {
            ApplyPresets();
        }

        public string FontAssetPresetId => _fontAssetPresetId;

        public FontAssetPreset GetFontAssetPreset()
        {
            return _fontAssetPresetId.IsNullOrEmpty()
                ? null
                : UiTextPresetsManager.Instance.FontAssetPresets[_fontAssetPresetId];
        }

        public void SetFontAssetPreset(string id)
        {
            _fontAssetPresetId = id;
        }

        public string TextPropertiesPresetId => _textPropertiesPresetId;

        public TextPropertiesPreset GetTextPropertiesPreset()
        {
            return _textPropertiesPresetId.IsNullOrEmpty()
                ? null
                : UiTextPresetsManager.Instance.TextPropertiesPresets[_textPropertiesPresetId];
        }

        public void SetTextPropertiesPreset(string id)
        {
            _textPropertiesPresetId = id;
        }

        public void ApplyPresets()
        {
            _text = GetComponent<TextMeshProUGUI>();

            var fontAssetPreset = GetFontAssetPreset();
            if (fontAssetPreset != null)
            {
                _text.font = fontAssetPreset.FontAsset;
                _text.fontSharedMaterial = fontAssetPreset.Material;
            }

            var textPropertiesPreset = GetTextPropertiesPreset();
            if (textPropertiesPreset != null)
            {
                _text.fontSize = textPropertiesPreset.FontSize;
                _text.color = textPropertiesPreset.FontColor;
            }
        }
    }
}
