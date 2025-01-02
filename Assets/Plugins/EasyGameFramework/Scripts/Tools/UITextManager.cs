using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using EasyGameFramework;
using EasyFramework;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pokemon.UI
{
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    public class UITextManager : MonoBehaviour
    {
        [InfoBoxCN("不要重置该组件(组件菜单栏的reset), 可能会导致引用的预设数据丢失!", InfoMessageType.Warning)]
        [ValueDropdown("GetFontAssetPresetDropdown")]
        [LabelText("字体资产预设")]
        public int FontAssetPresetIndex = -1;

        [ValueDropdown("GetUITextPropertiesPresetDropdown")]
        [LabelText("Text属性预设")]
        public int TextPropertiesPresetIndex = -1;

        public UITextPropertiesPreset TextPropertiesPreset
        {
            get
            {
                if (IsValidTextPropertiesPresetIndex)
                {
                    return UITextPresetsManager.Instance.UITextPropertiesPresets[TextPropertiesPresetIndex];
                }

                return null;
            }
        }

        public FontAssetPreset FontAssetPreset
        {
            get
            {
                if (IsValidFontAssetPresetIndex)
                {
                    return UITextPresetsManager.Instance.FontAssetPresets[FontAssetPresetIndex];
                }

                return null;
            }
        }

        private TextMeshProUGUI _text;

        private bool IsValidFontAssetPresetIndex =>
            FontAssetPresetIndex >= 0 && UITextPresetsManager.Instance.FontAssetPresets.Count > FontAssetPresetIndex;

        private bool IsValidTextPropertiesPresetIndex =>
            TextPropertiesPresetIndex >= 0 && UITextPresetsManager.Instance.UITextPropertiesPresets.Count >
            TextPropertiesPresetIndex;

        private void Awake()
        {
            ApplyPresets();
        }

        private void ApplyPresets()
        {
            if (_text == null)
            {
                _text = GetComponent<TextMeshProUGUI>();
                Debug.Assert(_text != null);
            }

            if (FontAssetPreset != null)
            {
                _text.font = FontAssetPreset.FontAsset;
                _text.fontSharedMaterial = FontAssetPreset.Material;
            }

            if (TextPropertiesPreset != null)
            {
                _text.fontSize = TextPropertiesPreset.FontSize;
                _text.color = TextPropertiesPreset.FontColor;
            }
        }

#if UNITY_EDITOR
        [TitleGroupCN("预设引用")]
        [SerializeField]
        //TODO ShowIf使用nameof优化
        [ShowIf(nameof(IsValidFontAssetPresetIndex))]
        [LabelText("字体资产预设-引用")]
        private FontAssetPreset _fontAssetPresetToShow;

        [TitleGroupCN("预设引用")]
        [SerializeField]
        [ShowIf(nameof(IsValidTextPropertiesPresetIndex))]
        [LabelText("Text属性预设-引用")]
        private UITextPropertiesPreset _textPropertiesPresetToShow;

        private IEnumerable GetFontAssetPresetDropdown()
        {
            var total = new ValueDropdownList<int>
            {
                { "None", -1 },
                { "-默认-", UITextPresetsManager.Instance.DefaultFontAssetPresetIndex }
            };
            total.AddRange(UITextPresetsManager.Instance.FontAssetPresets
                .Select((c, i) => new ValueDropdownItem<int>(c.Label, i)));
            return total;
        }

        private IEnumerable GetUITextPropertiesPresetDropdown()
        {
            var total = new ValueDropdownList<int> { { "None", -1 } };
            total.AddRange(UITextPresetsManager.Instance.UITextPropertiesPresets
                .Select((c, i) => new ValueDropdownItem<int>(c.Label, i)));
            return total;
        }

        private void Update()
        {
            if (UITextPresetsManager.Instance.RealtimePreview)
            {
                ApplyPresets();
            }
        }

        [Button("切换到预设管理器")]
        private void SelectManager()
        {
            Selection.activeObject = UITextPresetsManager.Instance;
            EditorGUIUtility.PingObject(UITextPresetsManager.Instance);
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            _fontAssetPresetToShow = FontAssetPreset;
            _textPropertiesPresetToShow = TextPropertiesPreset;
        }

        void Reset() {}
#endif
    }
}
