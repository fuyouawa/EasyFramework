using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using EasyFramework;
using JetBrains.Annotations;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyGameFramework
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class FontAssetPreset
    {
        [LabelText("字体资产")]
        [Required]
        [OnValueChanged("OnFontAssetChanged")]
        public TMP_FontAsset FontAsset;

        [Required]
        [LabelText("材质")]
        public Material Material;

        private void OnFontAssetChanged()
        {
            if (Material == null)
            {
                Material = FontAsset.material;
            }
        }
    }

    [Serializable]
    public class TextPropertiesPreset
    {
        [LabelText("字体大小")]
        public float FontSize;

        [LabelText("字体颜色")]
        public Color FontColor;
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
        [DictionaryDrawerSettings(KeyLabel = "标识", ValueLabel = "预设设置")]
        [LabelText("字体资产预设表")]
        public Dictionary<string, FontAssetPreset> FontAssetPresets = new();

        [TitleCN("文本属性")]
        [DictionaryDrawerSettings(KeyLabel = "标识", ValueLabel = "预设设置")]
        [LabelText("文本属性预设表")]
        public Dictionary<string, TextPropertiesPreset> TextPropertiesPresets = new();

        [LabelText("默认字体资产预设")]
        [ValueDropdown("FontAssetPresetDropdown")]
        [SerializeField]
        private string _defaultFontAssetPresetId;

        [LabelText("默认文本属性预设")]
        [ValueDropdown("TextPropertiesPresetDropdown")]
        [SerializeField]
        private string _defaultTextPropertiesPresetId;

        public string DefaultFontAssetPresetId => _defaultFontAssetPresetId;

        public string DefaultTextPropertiesPresetId => _defaultTextPropertiesPresetId;

#if UNITY_EDITOR
        private ValueDropdownList<string> _fontAssetPresetDropdown;
        private ValueDropdownList<string> _textPropertiesPresetDropdown;

        public ValueDropdownList<string> FontAssetPresetDropdown
        {
            get
            {
                //TODO FontAssetPresetDropdown性能优化
                _fontAssetPresetDropdown = new ValueDropdownList<string>();
                _fontAssetPresetDropdown.AddRange(FontAssetPresets.Select(p =>
                    new ValueDropdownItem<string>(p.Key.DefaultIfNullOrEmpty("TODO"), p.Key)));
                return _fontAssetPresetDropdown;
            }
        }

        public ValueDropdownList<string> TextPropertiesPresetDropdown
        {
            get
            {
                _textPropertiesPresetDropdown = new ValueDropdownList<string>();
                _textPropertiesPresetDropdown.AddRange(TextPropertiesPresets.Select(p =>
                    new ValueDropdownItem<string>(p.Key.DefaultIfNullOrEmpty("TODO"), p.Key)));
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
