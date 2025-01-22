using System;
using System.Collections.Generic;
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

        public Dictionary<string, FontAssetPreset> FontAssetPresets => _fontAssetPresets.Value;
        public Dictionary<string, TextPropertiesPreset> TextPropertiesPresets => _textPropertiesPresets.Value;

        public string DefaultFontAssetPresetId;
        public string DefaultTextPropertiesPresetId;

        public FontAssetPreset GetDefaultFontAssetPreset()
        {
            return _fontAssetPresets[DefaultFontAssetPresetId];
        }

        public TextPropertiesPreset GetDefaultTextPropertiesPreset()
        {
            return _textPropertiesPresets[DefaultTextPropertiesPresetId];
        }
    }
}
