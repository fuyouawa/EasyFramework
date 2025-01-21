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

    [ConfigAssetPath]
    public class UiTextPresetsManager : ScriptableObjectSingleton<UiTextPresetsManager>
    {
        public List<FontAssetPreset> FontAssetPresets = new List<FontAssetPreset>();
        public List<TextPropertiesPreset> TextPropertiesPresets = new List<TextPropertiesPreset>();
        public string DefaultFontAssetPresetId;
        public string DefaultTextPropertiesPresetId;


        public FontAssetPreset MatchFontAssetPreset(TMP_FontAsset fontAsset, Material material)
        {
            return FontAssetPresets.FirstOrDefault(p => p.FontAsset == fontAsset && p.Material == material);
        }

        public FontAssetPreset GetFontAssetPreset(string id)
        {
            return FontAssetPresets.FirstOrDefault(p => p.Id == id);
        }

        public TextPropertiesPreset MatchTextPropertiesPreset(float fontSize, Color fontColor)
        {
            return TextPropertiesPresets.FirstOrDefault(p =>
                p.FontSize.Approximately(fontSize) && p.FontColor == fontColor);
        }

        public TextPropertiesPreset GetTextPropertiesPreset(string id)
        {
            return TextPropertiesPresets.FirstOrDefault(p => p.Id == id);
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
