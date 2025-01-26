using TMPro;
using UnityEngine;

namespace EasyFramework
{
    public class UiTextManager : MonoBehaviour
    {
        [SerializeField] private string _fontAssetPresetId;
        [SerializeField] private string _textPropertiesPresetId;

        public string FontAssetPresetId
        {
            get => _fontAssetPresetId;
            set => _fontAssetPresetId = value;
        }

        public string TextPropertiesPresetId
        {
            get => _textPropertiesPresetId;
            set => _textPropertiesPresetId = value;
        }

        private TextMeshProUGUI _text;

        private void Awake()
        {
            ApplyPresets();
        }

        public FontAssetPreset GetFontAssetPreset()
        {
            return UiTextPresetsSettings.Instance.GetFontAssetPreset(FontAssetPresetId);
        }

        public TextPropertiesPreset GetTextPropertiesPreset()
        {
            return UiTextPresetsSettings.Instance.GetTextPropertiesPreset(TextPropertiesPresetId);
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
