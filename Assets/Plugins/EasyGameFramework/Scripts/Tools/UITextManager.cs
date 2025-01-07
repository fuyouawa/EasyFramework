using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace EasyGameFramework
{
    public class UiTextManager : SerializedMonoBehaviour
    {
        [ValueDropdown("FontAssetPresetDropdown")]
        [LabelText("字体资产预设")]
        [SerializeField]
        private int? _fontAssetPresetIndex;

        [ValueDropdown("TextPropertiesPresetDropdown")]
        [LabelText("文本属性预设")]
        [SerializeField]
        private int? _textPropertiesPresetIndex;

        private TextMeshProUGUI _text;

        private void Awake()
        {
            ApplyPresets();
        }

        public FontAssetPreset GetFontAssetPreset()
        {
            return  _fontAssetPresetIndex == null
                ? null
                : UiTextPresetsManager.Instance.FontAssetPresets[(int)_fontAssetPresetIndex];
        }

        public void SetFontAssetPreset(FontAssetPreset preset)
        {
            _fontAssetPresetIndex = UiTextPresetsManager.Instance.FontAssetPresets.IndexOf(preset);
            Changed();
        }

        public TextPropertiesPreset GetTextPropertiesPreset()
        {
            return  _textPropertiesPresetIndex == null
                ? null
                : UiTextPresetsManager.Instance.TextPropertiesPresets[(int)_textPropertiesPresetIndex];
        }

        public void SetTextPropertiesPreset(TextPropertiesPreset preset)
        {
            _textPropertiesPresetIndex = UiTextPresetsManager.Instance.TextPropertiesPresets.IndexOf(preset);
            Changed();
        }

        public void ApplyPresets()
        {
            _text = GetComponent<TextMeshProUGUI>();

            var fontAssetPreset = GetFontAssetPreset();
            if (fontAssetPreset != null)
            {
                if (_text.font != fontAssetPreset.FontAsset
                    || _text.fontSharedMaterial != fontAssetPreset.Material)
                {
                    Changed();
                }
                _text.font = fontAssetPreset.FontAsset;
                _text.fontSharedMaterial = fontAssetPreset.Material;

            }

            var textPropertiesPreset = GetTextPropertiesPreset();
            if (textPropertiesPreset != null)
            {
                if (!_text.fontSize.Approximately(textPropertiesPreset.FontSize)
                    || _text.color != textPropertiesPreset.FontColor)
                {
                    Changed();
                }
                _text.fontSize = textPropertiesPreset.FontSize;
                _text.color = textPropertiesPreset.FontColor;
            }
        }

        private void Changed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        private ValueDropdownList<int?> FontAssetPresetDropdown =>
            UiTextPresetsManager.Instance.FontAssetPresetDropdown;

        private ValueDropdownList<int?> TextPropertiesPresetDropdown =>
            UiTextPresetsManager.Instance.TextPropertiesPresetDropdown;
#endif
    }
}
