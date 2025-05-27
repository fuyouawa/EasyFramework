using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.UIKit.Editor
{
    public class TextStyleDrawer : OdinValueDrawer<TextStyle>
    {
        private InspectorProperty _nameProperty;
        private InspectorProperty _fontAssetProperty;
        private InspectorProperty _fontMaterialProperty;
        private InspectorProperty _fontSizeProperty;
        private InspectorProperty _fontColorProperty;

        private FoldoutGroupConfig _foldoutGroupConfig;

        protected override void Initialize()
        {
            _nameProperty = Property.Children["_name"];
            _fontAssetProperty = Property.Children["_fontAsset"];
            _fontMaterialProperty = Property.Children["_fontMaterial"];
            _fontSizeProperty = Property.Children["_fontSize"];
            _fontColorProperty = Property.Children["_fontColor"];

            _foldoutGroupConfig = new FoldoutGroupConfig(UniqueDrawerKey.Create(_nameProperty, this), new GUIContent(), _nameProperty.State.Expanded, OnContentGUI);
        }


        protected override void DrawPropertyLayout(GUIContent label)
        {
            _foldoutGroupConfig.Label.text = ValueEntry.SmartValue.Name.DefaultIfNullOrEmpty("TODO");
            _foldoutGroupConfig.Expand = _nameProperty.State.Expanded;
            _nameProperty.State.Expanded = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);
        }
        
        private void OnContentGUI(Rect headerRect)
        {
            _nameProperty.DrawEx("名称");
            _fontAssetProperty.DrawEx("字体资产");
            _fontMaterialProperty.DrawEx("字体材质");
            _fontSizeProperty.DrawEx("字体大小");
            _fontColorProperty.DrawEx("字体颜色");
        }
    }
}
