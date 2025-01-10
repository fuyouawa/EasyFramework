using EasyFramework;
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
        private string _fontAssetPresetId;

        [ValueDropdown("TextPropertiesPresetDropdown")]
        [LabelText("文本属性预设")]
        [SerializeField]
        private string _textPropertiesPresetId;

        private TextMeshProUGUI _text;

        private void Awake()
        {
            ApplyPresets();
        }

        public string FontAssetPresetId => _fontAssetPresetId;

        public FontAssetPreset GetFontAssetPreset()
        {
            return  _fontAssetPresetId.IsNullOrEmpty()
                ? null
                : UiTextPresetsManager.Instance.FontAssetPresets[_fontAssetPresetId];
        }

        public void SetFontAssetPreset(string id)
        {
            _fontAssetPresetId = id;
            Changed();
        }
        
        public string TextPropertiesPresetId => _textPropertiesPresetId;

        public TextPropertiesPreset GetTextPropertiesPreset()
        {
            return  _textPropertiesPresetId.IsNullOrEmpty()
                ? null
                : UiTextPresetsManager.Instance.TextPropertiesPresets[_textPropertiesPresetId];
        }

        public void SetTextPropertiesPreset(string id)
        {
            _textPropertiesPresetId = id;
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
        private ValueDropdownList<string> FontAssetPresetDropdown =>
            UiTextPresetsManager.Instance.FontAssetPresetDropdown;

        private ValueDropdownList<string> TextPropertiesPresetDropdown =>
            UiTextPresetsManager.Instance.TextPropertiesPresetDropdown;
#endif
    }
}
