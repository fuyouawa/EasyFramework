using TMPro;
using UnityEngine;

namespace EasyFramework.Tools
{
    public class UiTextManager : MonoBehaviour
    {
        public string FontAssetPresetId;
        public string TextPropertiesPresetId;

        private TextMeshProUGUI _text;

        private void Awake()
        {
            ApplyPresets();
        }

        public FontAssetPreset GetFontAssetPreset()
        {
            return UiTextPresetsManager.Instance.GetFontAssetPreset(FontAssetPresetId);
        }

        public TextPropertiesPreset GetTextPropertiesPreset()
        {
            return UiTextPresetsManager.Instance.GetTextPropertiesPreset(TextPropertiesPresetId);
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
