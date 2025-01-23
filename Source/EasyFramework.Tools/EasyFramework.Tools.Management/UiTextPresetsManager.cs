using System;
using System.Collections.Generic;
using EasyFramework.Generic;
using EasyFramework.Utilities;
using TMPro;
using UnityEngine;

namespace EasyFramework.Tools
{
    [Serializable]
    public class FontAssetPreset
    {
        public TMP_FontAsset FontAsset;
        public Material Material;
    }

    [Serializable]
    public class TextPropertiesPreset
    {
        public float FontSize = 26;
        public Color FontColor = Color.white;
    }

    [ConfigAssetPath]
    public class UiTextPresetsManager : ScriptableObjectSingleton<UiTextPresetsManager>
    {
        [SerializeField]
        private SerializedDictionary<string, FontAssetPreset> _fontAssetPresets =
            new SerializedDictionary<string, FontAssetPreset>();

        [SerializeField]
        private SerializedDictionary<string, TextPropertiesPreset> _textPropertiesPresets =
            new SerializedDictionary<string, TextPropertiesPreset>();

        [SerializeField] private string _defaultFontAssetPresetId;
        [SerializeField] private string _defaultTextPropertiesPresetId;


        public Dictionary<string, FontAssetPreset> FontAssetPresets => _fontAssetPresets.Value;
        public Dictionary<string, TextPropertiesPreset> TextPropertiesPresets => _textPropertiesPresets.Value;

        public string DefaultFontAssetPresetId
        {
            get => _defaultFontAssetPresetId;
            set => _defaultFontAssetPresetId = value;
        }

        public string DefaultTextPropertiesPresetId
        {
            get => _defaultTextPropertiesPresetId;
            set => _defaultTextPropertiesPresetId = value;
        }

        public FontAssetPreset GetFontAssetPreset(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return null;
            }

            if (_fontAssetPresets.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }

        public TextPropertiesPreset GetTextPropertiesPreset(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return null;
            }

            if (_textPropertiesPresets.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }

        public FontAssetPreset GetDefaultFontAssetPreset()
        {
            return GetFontAssetPreset(DefaultFontAssetPresetId);
        }

        public TextPropertiesPreset GetDefaultTextPropertiesPreset()
        {
            return GetTextPropertiesPreset(DefaultTextPropertiesPresetId);
        }
    }
}
