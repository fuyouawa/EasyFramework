using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using EasyGameFramework;
using EasyFramework;
using UnityEngine;

namespace Pokemon.UI
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
    public class UITextPropertiesPreset
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
    public class UITextPresetsManager : ScriptableObjectSingleton<UITextPresetsManager>
    {
        [LabelText("实时预览")]
        public bool RealtimePreview;

        [TitleCN("字体资产")]
        [ListDrawerSettings(ListElementLabelName = "LabelToShow", DraggableItems = false)]
        [LabelText("字体资产预设表")]
        public List<FontAssetPreset> FontAssetPresets = new();

        [LabelText("默认字体资产预设")]
        [ValueDropdown("GetFontAssetPresetDropdown")]
        public int DefaultFontAssetPresetIndex;

        [TitleCN("Text属性")]
        [ListDrawerSettings(ListElementLabelName = "LabelToShow", DraggableItems = false)]
        [LabelText("Text属性预设表")]
        public List<UITextPropertiesPreset> UITextPropertiesPresets = new();


#if UNITY_EDITOR
        public IEnumerable GetFontAssetPresetDropdown()
        {
            var total = new ValueDropdownList<int>();
            total.AddRange(FontAssetPresets.Select((c, i) => new ValueDropdownItem<int>(c.Label, i)));
            return total;
        }
#endif
    }
}
