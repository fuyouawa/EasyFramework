using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using EasyGameFramework;
using EasyFramework;
using JetBrains.Annotations;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyGameFramework
{
    [Serializable]
    public class FontAssetPreset
    {
        [LabelText("标签")]
        public string Label;

        [LabelText("字体资产")]
        public TMP_FontAsset FontAsset;

        [LabelText("材质")]
        public Material Material;

#if UNITY_EDITOR
        public string LabelToShow => Label.DefaultIfNullOrEmpty("TODO");
#endif
    }

    [Serializable]
    public class TextPropertiesPreset
    {
        [LabelText("标签")]
        public string Label;

        [LabelText("字体大小")]
        public float FontSize;

        [LabelText("字体颜色")]
        public Color FontColor;

#if UNITY_EDITOR
        public string LabelToShow => Label.DefaultIfNullOrEmpty("TODO");
#endif
    }

    [ScriptableObjectSingletonAssetPath("Assets/Resources/Fonts")]
    [ShowOdinSerializedPropertiesInInspector]
    public class UiTextPresetsManager : ScriptableObjectSingleton<UiTextPresetsManager>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private SerializationData _serializationData;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this._serializationData);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref this._serializationData);
        }

        [TitleCN("字体资产")]
        [ListDrawerSettings(ListElementLabelName = "LabelToShow", DraggableItems = false)]
        [LabelText("字体资产预设表")]
        public List<FontAssetPreset> FontAssetPresets = new();

        [TitleCN("文本属性")]
        [ListDrawerSettings(ListElementLabelName = "LabelToShow", DraggableItems = false, CustomAddFunction = "OnAddTextPropertiesPreset")]
        [LabelText("文本属性预设表")]
        public List<TextPropertiesPreset> TextPropertiesPresets = new();
        
        [LabelText("默认字体资产预设")]
        [ValueDropdown("FontAssetPresetDropdown")]
        [SerializeField]
        private int _defaultFontAssetPresetIndex = -1;

        [LabelText("默认文本属性预设")]
        [ValueDropdown("TextPropertiesPresetDropdown")]
        [SerializeField]
        private int _defaultTextPropertiesPresetIndex = -1;

        public FontAssetPreset GetDefaultFontAssetPreset()
        {
            return _defaultFontAssetPresetIndex == -1 ? null : FontAssetPresets[(int)_defaultFontAssetPresetIndex];
        }

        public TextPropertiesPreset GetDefaultTextPropertiesPreset()
        {
            return _defaultTextPropertiesPresetIndex == -1 ? null : TextPropertiesPresets[(int)_defaultTextPropertiesPresetIndex];
        }

#if UNITY_EDITOR
        private ValueDropdownList<int> _fontAssetPresetDropdown;
        private ValueDropdownList<int> _textPropertiesPresetDropdown;

        public ValueDropdownList<int> FontAssetPresetDropdown
        {
            get
            {
                //TODO FontAssetPresetDropdown性能优化
                _fontAssetPresetDropdown = new ValueDropdownList<int> { { "None", -1 } };
                _fontAssetPresetDropdown.AddRange(FontAssetPresets.Select((p, i) => new ValueDropdownItem<int>(p.LabelToShow, i)));
                return _fontAssetPresetDropdown;
            }
        }
        public ValueDropdownList<int> TextPropertiesPresetDropdown
        {
            get
            {
                _textPropertiesPresetDropdown = new ValueDropdownList<int> { { "None", -1 } };
                _textPropertiesPresetDropdown.AddRange(TextPropertiesPresets.Select((p, i) => new ValueDropdownItem<int>(p.LabelToShow, i)));
                return _textPropertiesPresetDropdown;
            }
        }

        [UsedImplicitly]
        private TextPropertiesPreset OnAddTextPropertiesPreset()
        {
            var preset = new TextPropertiesPreset
            {
                FontSize = 50,
                FontColor = Color.white
            };
            return preset;
        }
#endif
    }
}
