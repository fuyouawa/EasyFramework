using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Utilities;
using TMPro;
using UnityEngine;

namespace EasyFramework.Tools
{
    [Serializable]
    public class FontAssetPreset
    {
        public string Id = "TODO";
        public TMP_FontAsset FontAsset;
        public Material Material;
    }

    [Serializable]
    public class TextPropertiesPreset
    {
        public string Id = "TODO";
        public float FontSize = 26;
        public Color FontColor = Color.white;
    }

    [ScriptableObjectSingletonAssetPath("Assets/Resources/Fonts")]
    public class UiTextPresetsManager : ScriptableObjectSingleton<UiTextPresetsManager>
    {
        [SerializeField] private List<FontAssetPreset> _fontAssetPresets = new List<FontAssetPreset>();
        [SerializeField] private List<TextPropertiesPreset> _textPropertiesPresets = new List<TextPropertiesPreset>();
        [SerializeField] private string _defaultFontAssetPresetId;
        [SerializeField] private string _defaultTextPropertiesPresetId;

        public List<FontAssetPreset> FontAssetPresets => _fontAssetPresets;
        public List<TextPropertiesPreset> TextPropertiesPresets => _textPropertiesPresets;

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

        public FontAssetPreset MatchFontAssetPreset(TMP_FontAsset fontAsset, Material material)
        {
            return _fontAssetPresets.FirstOrDefault(p => p.FontAsset == fontAsset && p.Material == material);
        }

        public FontAssetPreset GetFontAssetPreset(string id)
        {
            return _fontAssetPresets.FirstOrDefault(p => p.Id == id);
        }

        public TextPropertiesPreset MatchTextPropertiesPreset(float fontSize, Color fontColor)
        {
            return _textPropertiesPresets.FirstOrDefault(p =>
                p.FontSize.Approximately(fontSize) && p.FontColor == fontColor);
        }

        public TextPropertiesPreset GetTextPropertiesPreset(string id)
        {
            return _textPropertiesPresets.FirstOrDefault(p => p.Id == id);
        }

        public FontAssetPreset GetDefaultFontAssetPreset()
        {
            return GetFontAssetPreset(_defaultFontAssetPresetId);
        }

        public TextPropertiesPreset GetDefaultTextPropertiesPreset()
        {
            return GetTextPropertiesPreset(_defaultTextPropertiesPresetId);
        }
    }
}
